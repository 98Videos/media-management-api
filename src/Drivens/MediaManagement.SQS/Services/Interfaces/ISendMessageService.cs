namespace MediaManagement.SQS.Adapters.Interfaces;

public interface ISendMessageService
{
    Task SendMessageAsync<T>(T message, CancellationToken cancellationToken) where T : class;
}