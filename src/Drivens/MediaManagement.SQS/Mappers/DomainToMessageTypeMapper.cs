using MediaManagement.SQS.Contracts;
using MediaManagementApi.Domain.Entities;

namespace MediaManagement.SQS.Mappers
{
    internal class VideoMessageMapper : IDomainToMessageMapper<Video>
    {
        object IDomainToMessageMapper<Video>.MapToMessageContract(Video source)
        {
            return new VideoToProcessMessage()
            {
                UserEmail = source.EmailUser,
                VideoId = source.Filename,
            };
        }
    }
}