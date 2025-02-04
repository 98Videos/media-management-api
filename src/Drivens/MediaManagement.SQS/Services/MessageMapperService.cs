using MediaManagement.SQS.Adapters.Interfaces;

namespace MediaManagement.SQS.Mappers;

public class MessageMapperService: IMessageMapperService
{
    public object MapToMessage<T>(T obj) where T : class
    {
        var mapper = DomainToMessageMapperFactory.GetMessageMapper(obj);
        return mapper?.MapToMessageContract(obj);
    }
}