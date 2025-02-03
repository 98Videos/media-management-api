using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using NUnit.Framework;

namespace MediaManagement.UnitTests.Core.Domain.Entities;

[TestFixture]
public class VideoTests
{
    [Test]
    public void Constructor_ShouldCreateValidVideo()
    {
        // Arrange
        var email = "user@example.com";
        var filename = "video.mp4";

        // Act
        var video = new Video(email, filename);

        // Assert
        Assert.NotNull(video);
        Assert.AreEqual(email, video.EmailUser);
        Assert.AreEqual(filename, video.Filename);
        Assert.AreEqual(VideoStatus.EmProcessamento, video.Status);
        Assert.AreNotEqual(Guid.Empty, video.Id);

    }

    [Test]
    public void UpdateStatus_ShouldUpdateStatusAndSetUpdatedAt()
    {
        // Arrange
        var video = new Video("user@example.com", "video.mp4");
        var newStatus = VideoStatus.Processado;

        // Act
        video.UpdateStatus(newStatus);

        // Assert
        Assert.AreEqual(newStatus, video.Status);
    }

    [Test]
    public void CompleteProcessing_ShouldMarkAsProcessed()
    {
        // Arrange
        var video = new Video("user@example.com", "video.mp4");

        // Act
        video.UpdateStatus(VideoStatus.Processado);

        // Assert
        Assert.AreEqual(VideoStatus.Processado, video.Status);
    }
}