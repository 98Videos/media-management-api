using MassTransit;
using MediaManagement.SQS.Contracts;
using MediaManagement.SQS.Options;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaManagement.SQS.Adapters
{
    public class SQSMessagePublisher : IMessagePublisher
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<SQSMessagePublisher> _logger;
        private readonly SqsMessagePublisherOptions _options;

        public SQSMessagePublisher(
            ISendEndpointProvider sendEndpointProvider,
            IOptions<SqsMessagePublisherOptions> options,
            ILogger<SQSMessagePublisher> logger)
        {
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value;
        }

        public async Task PublishVideoToProcessMessage(Video video, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(video);

            try
            {
                var message = new VideoToProcessMessage()
                {
                    UserEmail = video.EmailUser,
                    VideoId = video.Id.ToString(),
                };

                var queueUri = new Uri($"queue:{_options.QueueName}");
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(queueUri);

                await endpoint.Send(message, cancellationToken);
                _logger.LogInformation("Published message for video {videoId}", video.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while publishing the message for video {videoId}", video.Id.ToString());
                throw;
            }
        }
    }
}