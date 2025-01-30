using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Repositories;
using System;

namespace MediaManagement.Application.UseCases;

public class VideoUseCase : IVideoUseCase
{
    private readonly IVideoRepository _videoRepository;
    private readonly IFileRepository _fileRepository;

    public VideoUseCase(IVideoRepository videoRepository, IFileRepository fileRepository)
    {
        _videoRepository = videoRepository;
        _fileRepository = fileRepository;
    }

    public async Task<Video> ExecuteAsync(string emailUser, Stream stream, string fileName)
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
            VideoStatus.NAO_PROCESSADO);

        try
        {
            await _fileRepository.UploadVideoFile(emailUser, fileName, stream);

            Video savedVideo = await _videoRepository.AddAsync(video);

            return savedVideo;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao processar o vídeo {fileName} para o usuário {emailUser}.", ex);
        }
    }

    public async Task<Video> UpdateStatus(Guid videoId)
    {
        if (videoId == Guid.Empty)
        {
            throw new ArgumentException("O ID do vídeo não pode ser vazio.", nameof(videoId));
        }

        Video video = await _videoRepository.GetVideoAsync(videoId);
        
        if (video == null)
        {
            throw new KeyNotFoundException($"Vídeo com o ID {videoId} não encontrado.");
        }

        video.UpdateStatus(VideoStatus.PROCESSADO);
        await _videoRepository.UpdateAsync(video);

        return video;
    }

    public async Task<IEnumerable<Video>> GetAllVideosByUser(string emailUser)
    {
        var list = await _videoRepository.GetAllVideosByUserAsync(emailUser);
        if (list is null || !list.Any())
        {
            throw new KeyNotFoundException();
        }
        return list;
    }
}