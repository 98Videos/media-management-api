using MassTransit;
using MediaManagement.SQS.Adapters;
using MediaManagement.SQS.Contracts;
using MediaManagement.SQS.Options;
using MediaManagementApi.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace MediaManagement.UnitTests.Drivens.SQS.Adapters
{
    [TestFixture]
    public class SQSMessagePublisherTests
    {
        private readonly Mock<ISendEndpointProvider> _sendEndpointProviderMock = new();
        private readonly Mock<ISendEndpoint> _sendEndpointMock = new();
        private readonly Mock<ILogger<SQSMessagePublisher>> _loggerMock = new();
        private readonly SqsMessagePublisherOptions _options = new() { QueueName = "test" };
        private SQSMessagePublisher _publisher = null!;
        [SetUp]
        public void SetUp()
        {
            _sendEndpointMock.Reset();
            _sendEndpointProviderMock.Reset();
            _loggerMock.Reset();

            _sendEndpointProviderMock
                .Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
                .ReturnsAsync(_sendEndpointMock.Object);

            _publisher = new SQSMessagePublisher(
                _sendEndpointProviderMock.Object,
                Options.Create(_options),
                _loggerMock.Object);
        }

        [Test]
        public async Task PublishAsync_WhenReceivingAVideo_ShouldCallSendWithCorrectMessage()
        {
            // Arrange
            var receivedVideo = new Video("email@test.com", "testvideo.mp4");

            // Act
            await _publisher.PublishVideoToProcessMessage(receivedVideo);

            // Assert
            _sendEndpointMock.Verify(m => m.Send(It.Is<VideoToProcessMessage>(x =>
                    x.VideoId == receivedVideo.Id.ToString()
                    && x.UserEmail == receivedVideo.EmailUser
                ),
                It.IsAny<CancellationToken>()),
            Times.Once);
        }

        [Test]
        public async Task PublishAsync_WhenOptionsIsPopulated_ShouldCallGetSendEndpointWithCorrectUri()
        {
            // Arrange
            var receivedVideo = new Video("email@test.com", "testvideo.mp4");

            // Act
            await _publisher.PublishVideoToProcessMessage(receivedVideo);

            // Assert
            _sendEndpointProviderMock
                .Verify(x =>
                x.GetSendEndpoint(It.Is<Uri>(x =>
                    x.OriginalString == $"queue:{_options.QueueName}"
                )),
                Times.Once);
        }

        [Test]
        public void PublishAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var receivedVideo = new Video("email@test.com", "testvideo.mp4");

            _sendEndpointMock
                .Setup(s => s.Send(It.IsAny<VideoToProcessMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _publisher.PublishVideoToProcessMessage(receivedVideo));

            Assert.That(ex!.Message, Is.EqualTo("Simulated exception"));

            _loggerMock.Verify(
                log => log.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, type) => obj.ToString()!.Contains($"An error occured while publishing the message for video {receivedVideo.Id}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}