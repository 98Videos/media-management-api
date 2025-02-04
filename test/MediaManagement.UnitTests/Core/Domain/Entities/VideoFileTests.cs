using MediaManagementApi.Domain.Entities;
using NUnit.Framework;

namespace MediaManagement.UnitTests.Core.Domain.Entities;

public class VideoFileTests
{
    [Test]
    public void VideoFile_ShouldInitializeWithGivenValues()
    {
        string expectedIdentifier = "test-video";
        using var memoryStream = new MemoryStream();

        var videoFile = new VideoFile(expectedIdentifier, memoryStream);

        Assert.AreEqual(expectedIdentifier, videoFile.Identifier);
        Assert.AreEqual(memoryStream, videoFile.FileStreamReference);
    }
}