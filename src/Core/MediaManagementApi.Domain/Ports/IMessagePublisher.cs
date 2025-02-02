namespace MediaManagementApi.Domain.Ports
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message) where T : class;
    }
}