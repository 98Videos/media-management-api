using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Ports;
using MediaManagementApi.Domain.Repositories;

namespace MediaManagement.Application.UseCases;

public class VideoUseCase : IVideoUseCase
{
    private readonly IVideoRepository _videoRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IMessagePublisher _messagePublisher;

    public VideoUseCase(IVideoRepository videoRepository, IFileRepository fileRepository, IMessagePublisher messagePublisher)
    {
        _videoRepository = videoRepository;
        _fileRepository = fileRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<Video> ExecuteAsync(string emailUser, Stream stream, string fileName, CancellationToken cancellationToken)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream), "O stream não pode ser nulo.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("O nome do arquivo não pode ser nulo ou vazio.", nameof(fileName));
        }

        Video video = new Video(
            emailUser: emailUser,
            filename: fileName,
            VideoStatus.EmProcessamento);

        try
        {
            var savedVideo = await _videoRepository.AddAsync(video, cancellationToken);

            await _fileRepository.UploadVideoFileAsync(emailUser, fileName, stream, cancellationToken);

            await _messagePublisher.PublishAsync(video, cancellationToken);

            return savedVideo;
        }
        catch (Exception ex)
        {
            video.UpdateStatus(VideoStatus.Falha);
            throw new InvalidOperationException($"Erro ao processar o vídeo {fileName} para o usuário {emailUser}.", ex);
        }
    }

    public async Task<Video> UpdateStatusAsync(Guid videoId, VideoStatus status, CancellationToken cancellationToken)
    {
        if (videoId == Guid.Empty)
        {
            throw new ArgumentException("O ID do vídeo não pode ser vazio.", nameof(videoId));
        }

        var video = await _videoRepository.GetVideoAsync(videoId, cancellationToken) 
            ?? throw new KeyNotFoundException($"Vídeo com o ID {videoId} não encontrado.");

        video.UpdateStatus(status);
        await _videoRepository.UpdateAsync(video, cancellationToken);

        return video;
    }

    public async Task<IEnumerable<Video>> GetAllVideosByUserAsync(string emailUser, CancellationToken cancellationToken = default)
    {
        var list = await _videoRepository.GetAllVideosByUserAsync(emailUser, cancellationToken);
        if (list is null || !list.Any())
        {
            throw new KeyNotFoundException();
        }
        return list;
    }
}