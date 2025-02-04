using MediaManagement.SQS.Adapters;
using MediaManagement.SQS.Adapters.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MediaManagement.Tests
{
    [TestFixture]
    public class SQSMessagePublisherTests
    {
        private Mock<ISendMessageService> _sendMessageServiceMock;
        private Mock<IMessageMapperService> _messageMapperServiceMock;
        private Mock<ILogger<SQSMessagePublisher>> _loggerMock;
        private SQSMessagePublisher _publisher;

        [SetUp]
        public void SetUp()
        {
            _sendMessageServiceMock = new Mock<ISendMessageService>();
            _messageMapperServiceMock = new Mock<IMessageMapperService>();
            _loggerMock = new Mock<ILogger<SQSMessagePublisher>>();

            _publisher = new SQSMessagePublisher(
                _sendMessageServiceMock.Object,
                _messageMapperServiceMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task PublishAsync_ShouldCallSendMessage_WhenMappingIsSuccessful()
        {
            // Arrange
            var testMessage = new { Id = 1, Name = "Test" };
            var mappedMessage = new { ProcessedId = 1, ProcessedName = "MappedTest" };

            _messageMapperServiceMock
                .Setup(m => m.MapToMessage(testMessage))
                .Returns(mappedMessage);

            _sendMessageServiceMock
                .Setup(s => s.SendMessageAsync(mappedMessage, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _publisher.PublishAsync(testMessage);

            // Assert
            _messageMapperServiceMock.Verify(m => m.MapToMessage(testMessage), Times.Once);
            _sendMessageServiceMock.Verify(s => s.SendMessageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void PublishAsync_ShouldThrowException_WhenMappingFails()
        {
            // Arrange
            var testMessage = new { Id = 1, Name = "Test" };

            _messageMapperServiceMock
                .Setup(m => m.MapToMessage(testMessage))
                .Returns((object)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await _publisher.PublishAsync(testMessage));

            Assert.That(ex.Message, Is.EqualTo($"Mapped message is null for {testMessage.GetType().Name}"));

            _messageMapperServiceMock.Verify(m => m.MapToMessage(testMessage), Times.Once);
            _sendMessageServiceMock.Verify(s => s.SendMessageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task PublishAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var testMessage = new { Id = 1, Name = "Test" };
            var mappedMessage = new { ProcessedId = 1, ProcessedName = "MappedTest" };

            _messageMapperServiceMock
                .Setup(m => m.MapToMessage(testMessage))
                .Returns(mappedMessage);

            _sendMessageServiceMock
                .Setup(s => s.SendMessageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _publisher.PublishAsync(testMessage));

            Assert.That(ex.Message, Is.EqualTo("Simulated exception"));

            _loggerMock.Verify(
                log => log.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, type) => obj.ToString().Contains("Error sending message of type")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
