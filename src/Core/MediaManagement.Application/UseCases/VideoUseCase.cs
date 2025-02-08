using MediaManagement.Application.Constants;
using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Ports;
using MediaManagementApi.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace MediaManagement.Application.UseCases;

public class VideoUseCase : IVideoUseCase
{
    private readonly IVideoRepository _videoRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly INotificationSender _notificationSender;
    private readonly ILogger<VideoUseCase> _logger;

    public VideoUseCase(IVideoRepository videoRepository,
                        IFileRepository fileRepository,
                        IMessagePublisher messagePublisher,
                        INotificationSender notificationSender,
                        ILogger<VideoUseCase> logger)
    {
        _videoRepository = videoRepository;
        _fileRepository = fileRepository;
        _messagePublisher = messagePublisher;
        _notificationSender = notificationSender;
        _logger = logger;
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

        var video = new Video(emailUser: emailUser, filename: fileName);

        try
        {
            var savedVideo = await _videoRepository.AddAsync(video, cancellationToken);

            await _fileRepository.UploadVideoFileAsync(emailUser, video.Id.ToString(), stream, cancellationToken);

            await _messagePublisher.PublishVideoToProcessMessage(video, cancellationToken);

            return savedVideo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error while uploading video {fileName}", fileName);

            video.UpdateStatus(VideoStatus.Falha);
            await _videoRepository.UpdateAsync(video, cancellationToken);

            await _notificationSender.SendNotification(NotificationConstants.StatusUpdateSubject, video.EmailUser, NotificationConstants.VideoProcessingFailed);

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

        var message = video.Status switch {
            VideoStatus.EmProcessamento => NotificationConstants.VideoNotDoneProcessing,
            VideoStatus.Processado => NotificationConstants.VideoProcessedSuccesfully,
            VideoStatus.Falha => NotificationConstants.VideoProcessingFailed,
            _ => throw new InvalidOperationException()
        };
        await _notificationSender.SendNotification(NotificationConstants.StatusUpdateSubject, video.EmailUser, message);

        return video;
    }

    public async Task<IEnumerable<Video>> GetAllVideosByUserAsync(string emailUser, CancellationToken cancellationToken = default)
    {
        var list = await _videoRepository.GetAllVideosByUserAsync(emailUser, cancellationToken);
        return list;
    }
}