using MassTransit;
using MediaManagementApi.Domain.Ports;

namespace MediaManagement.SQS.Adapters
{
    public class SQSMessagePublisher<T> : IMessagePublisher<T>
        where T : class
    {
        private readonly IBus messageBus;

        public SQSMessagePublisher(IBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task PublishAsync(T message)
        {
            await messageBus.Publish(message);
        }
    }
}
