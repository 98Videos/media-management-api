using MediaManagement.SQS.Adapters.Interfaces;
using MediaManagementApi.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace MediaManagement.SQS.Adapters
{
    public class SQSMessagePublisher : IMessagePublisher
    {
        private readonly ISendMessageService _sendMessageService;
        private readonly IMessageMapperService _messageMapperService;
        private readonly ILogger<SQSMessagePublisher> _logger;

        public SQSMessagePublisher(
            ISendMessageService sendMessageService,
            IMessageMapperService messageMapperService,
            ILogger<SQSMessagePublisher> logger)
        {
            _sendMessageService = sendMessageService ?? throw new ArgumentNullException(nameof(sendMessageService));
            _messageMapperService = messageMapperService ?? throw new ArgumentNullException(nameof(messageMapperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<T>(T obj, CancellationToken cancellationToken = default) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                var message = _messageMapperService.MapToMessage(obj);
                if (message == null)
                {
                    _logger.LogError("Mapped message is null for {MessageType}", typeof(T).Name);
                    throw new InvalidOperationException($"Mapped message is null for {typeof(T).Name}");
                }

                await _sendMessageService.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation("Published message of type {MessageType} successfully", typeof(T).Name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending message of type {MessageType}", typeof(T).Name);
                throw;
            }
        }
    }
}
