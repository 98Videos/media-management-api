using Amazon.S3;
using MediaManagement.S3.Adapters;
using MediaManagement.S3.Exceptions;
using MediaManagement.S3.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace MediaManagement.UnitTests.Drivens.S3.Adapters;

public class S3FileRepositoryTests
{
    private Mock<IAmazonS3> _mockS3Client;
    private Mock<ILogger<S3FileRepository>> _mockLogger;
    private Mock<IOptions<S3BucketOptions>> _mockOptions;
    private S3FileRepository _fileRepository;
    private const string UserEmail = "testuser@example.com";
    private const string FileIdentifier = "video123";

    [SetUp]
    public void SetUp()
    {
        _mockS3Client = new Mock<IAmazonS3>();
        _mockLogger = new Mock<ILogger<S3FileRepository>>();
        _mockOptions = new Mock<IOptions<S3BucketOptions>>();
        _mockOptions.Setup(o => o.Value).Returns(new S3BucketOptions
        {
            VideosBucket = "test-bucket"
        });

        _fileRepository = new S3FileRepository(
            _mockS3Client.Object,
            _mockOptions.Object,
            _mockLogger.Object
        );
    }

    [Test]
    public async Task GetVideoFileAsync_Success_ReturnsVideoFile()
    {
        // Arrange
        var s3Response = new Amazon.S3.Model.GetObjectResponse
        {
            ResponseStream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 }) // Dummy stream
        };
        _mockS3Client
            .Setup(client => client.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(s3Response);

        // Act
        var result = await _fileRepository.GetVideoFileAsync(UserEmail, FileIdentifier);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(FileIdentifier, result.Identifier);
        Assert.AreEqual(s3Response.ResponseStream, result.FileStreamReference);
        _mockS3Client.Verify(client => client.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void GetVideoFileAsync_InvalidUserEmail_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _fileRepository.GetVideoFileAsync(null, FileIdentifier));
        Assert.ThrowsAsync<ArgumentException>(async () => await _fileRepository.GetVideoFileAsync(string.Empty, FileIdentifier));
    }

    [Test]
    public void UploadVideoFileAsync_InvalidEmail_ThrowsArgumentException()
    {
        // Arrange
        var videoStream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 });

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _fileRepository.UploadVideoFileAsync(null, FileIdentifier, videoStream));
        Assert.ThrowsAsync<ArgumentException>(async () => await _fileRepository.UploadVideoFileAsync(string.Empty, FileIdentifier, videoStream));
    }

    [Test]
    public void UploadVideoFileAsync_EmptyStream_ThrowsArgumentException()
    {
        // Arrange
        var emptyStream = new MemoryStream();

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _fileRepository.UploadVideoFileAsync(UserEmail, FileIdentifier, emptyStream));
    }

    [Test]
    public async Task UploadVideoFileAsync_Error_ThrowsFileUploadException()
    {
        // Arrange
        var videoStream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 });
        _mockS3Client
            .Setup(client => client.PutObjectAsync(It.IsAny<Amazon.S3.Model.PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("S3 error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<FileUploadException>(async () => await _fileRepository.UploadVideoFileAsync(UserEmail, FileIdentifier, videoStream));
        Assert.That(ex.Message, Is.EqualTo($"Error uploading file {FileIdentifier} for user {UserEmail}"));
    }
}