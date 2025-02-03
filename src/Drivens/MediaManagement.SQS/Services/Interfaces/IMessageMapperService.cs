namespace MediaManagement.SQS.Adapters.Interfaces;

public interface IMessageMapperService
{
    object MapToMessage<T>(T obj) where T : class;
}