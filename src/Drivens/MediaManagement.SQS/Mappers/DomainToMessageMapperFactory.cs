using MediaManagementApi.Domain.Entities;

namespace MediaManagement.SQS.Mappers
{
    internal static class DomainToMessageMapperFactory
    {
        public static IDomainToMessageMapper<TSource> GetMessageMapper<TSource>(TSource source)
            where TSource : class

        {
            return source switch
            {
                Video => (IDomainToMessageMapper<TSource>) new VideoMessageMapper(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}