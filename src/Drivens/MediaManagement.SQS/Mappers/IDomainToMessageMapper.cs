namespace MediaManagement.SQS.Mappers
{
    internal interface IDomainToMessageMapper<TSource>
    {
        object MapToMessageContract(TSource source);
    }
}