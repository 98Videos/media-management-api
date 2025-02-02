using MassTransit;
using MediaManagement.SQS.Mappers;
using MediaManagementApi.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace MediaManagement.SQS.Adapters
{
    public class SQSMessagePublisher : IMessagePublisher
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<SQSMessagePublisher> _logger;

        public SQSMessagePublisher(ISendEndpointProvider sendEndpointProvider, ILogger<SQSMessagePublisher> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T obj)
            where T : class 
        {
            try
            {
                var mapper = DomainToMessageMapperFactory.GetMessageMapper(obj);
                var message = mapper.MapToMessageContract(obj);

                await _sendEndpointProvider.Send(message);
                _logger.LogInformation("published message successfully");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error sending message");
                throw;
            }
        }
    }
}