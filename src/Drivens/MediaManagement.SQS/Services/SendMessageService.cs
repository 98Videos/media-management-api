using MassTransit;
using MediaManagement.SQS.Adapters.Interfaces;

namespace MediaManagement.SQS.Services
{
    internal class SendMessageService : ISendMessageService
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public SendMessageService(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task SendMessageAsync<T>(T message, CancellationToken cancellationToken) where T : class => 
            await _sendEndpointProvider.Send(message, cancellationToken);
    }
}
