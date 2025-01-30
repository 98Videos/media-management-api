namespace MediaManagementApi.Domain.Ports
{
    public interface IMessagePublisher<T>
        where T : class
    {
        Task PublishAsync(T message);
    }
}
