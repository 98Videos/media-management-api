using MediaManagement.Application.Constants;
using MediaManagement.Application.UseCases;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Ports;
using MediaManagementApi.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MediaManagement.UnitTests.Core.Application.UseCases;

[TestFixture]
public class VideoUseCaseTests
{
    private Mock<IVideoRepository> _videoRepositoryMock;
    private Mock<IFileRepository> _fileRepositoryMock;
    private Mock<IMessagePublisher> _messagePublisherMock;
    private Mock<INotificationSender> _notificationSenderMock;
    private Mock<ILogger<VideoUseCase>> _loggerMock;
    private VideoUseCase _videoUseCase;

    [SetUp]
    public void SetUp()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _fileRepositoryMock = new Mock<IFileRepository>();
        _messagePublisherMock = new Mock<IMessagePublisher>();
        _notificationSenderMock = new Mock<INotificationSender>();
        _loggerMock = new Mock<ILogger<VideoUseCase>>();
        _videoUseCase = new VideoUseCase(_videoRepositoryMock.Object, _fileRepositoryMock.Object, _messagePublisherMock.Object, _notificationSenderMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_ShouldThrowException_WhenStreamIsNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _videoUseCase.ExecuteAsync("user@example.com", null, "video.mp4", CancellationToken.None));
    }

    [Test]
    public async Task ExecuteAsync_ShouldThrowException_WhenFileNameIsEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _videoUseCase.ExecuteAsync("user@example.com", new MemoryStream(), "", CancellationToken.None));
    }

    [Test]
    public async Task ExecuteAsync_ShouldCallRepositoriesAndReturnVideo()
    {
        var emailUser = "user@example.com";
        var fileName = "video.mp4";
        var stream = new MemoryStream();
        var video = new Video(emailUser, fileName);

        _videoRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Video>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        
        var result = await _videoUseCase.ExecuteAsync(emailUser, stream, fileName, CancellationToken.None);

        _videoRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Video>(), It.IsAny<CancellationToken>()), Times.Once);
        _fileRepositoryMock.Verify(repo => repo.UploadVideoFileAsync(emailUser, It.IsAny<string>(), stream, It.IsAny<CancellationToken>()), Times.Once);
        _messagePublisherMock.Verify(pub => pub.PublishVideoToProcessMessage(It.IsAny<Video>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.AreEqual(video, result);
    }

    [Test]
    public async Task ExecuteAsync_WhenAnErrorOccurs_ShouldUpdateStatusToFailureAndSendNotification()
    {
        var emailUser = "user@example.com";
        var fileName = "video.mp4";
        var stream = new MemoryStream();
        var video = new Video(emailUser, fileName);

        _videoRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Video>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);

        _fileRepositoryMock.Setup(repo => repo.UploadVideoFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var result = await _videoUseCase.ExecuteAsync(emailUser, stream, fileName, CancellationToken.None);

            _videoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Video>(x => x.Status == VideoStatus.Falha), It.IsAny<CancellationToken>()), Times.Once);
            _notificationSenderMock.Verify(x => x.SendNotification(NotificationConstants.StatusUpdateSubject, emailUser, NotificationConstants.VideoProcessingFailed), Times.Once);
        });
    }

    [Test]
    public async Task UpdateStatusAsync_ShouldThrowException_WhenVideoIdIsEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _videoUseCase.UpdateStatusAsync(Guid.Empty, VideoStatus.EmProcessamento, CancellationToken.None));
    }

    [Test]
    public async Task UpdateStatusAsync_ShouldUpdateVideoStatus()
    {
        var videoId = Guid.NewGuid();
        var video = new Video("user@example.com", "video.mp4");

        _videoRepositoryMock.Setup(repo => repo.GetVideoAsync(videoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);

        var result = await _videoUseCase.UpdateStatusAsync(videoId, VideoStatus.Processado, CancellationToken.None);

        _videoRepositoryMock.Verify(repo => repo.UpdateAsync(video, It.IsAny<CancellationToken>()), Times.Once);
        Assert.AreEqual(VideoStatus.Processado, result.Status);
    }

    [TestCase(VideoStatus.Processado, NotificationConstants.VideoProcessedSuccesfully)]
    [TestCase(VideoStatus.Falha, NotificationConstants.VideoProcessingFailed)]
    public async Task UpdateStatusAsync_ShouldSendNotificationWithNewStatus(VideoStatus newStatus, string expectedMessage)
    {
        var videoId = Guid.NewGuid();
        var video = new Video("user@example.com", "video.mp4");

        _videoRepositoryMock.Setup(repo => repo.GetVideoAsync(videoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);

        var result = await _videoUseCase.UpdateStatusAsync(videoId, newStatus, CancellationToken.None);

        _notificationSenderMock.Verify(x => x.SendNotification(NotificationConstants.StatusUpdateSubject, video.EmailUser, expectedMessage));
    }

    [Test]
    public async Task GetAllVideosByUserAsync_ShouldReturnVideos()
    {
        var emailUser = "user@example.com";
        var videos = new List<Video> { new Video(emailUser, "video1.mp4"), new Video(emailUser, "video2.mp4") };

        _videoRepositoryMock.Setup(repo => repo.GetAllVideosByUserAsync(emailUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(videos);

        var result = await _videoUseCase.GetAllVideosByUserAsync(emailUser, CancellationToken.None);

        Assert.AreEqual(2, result.Count());
    }
}
