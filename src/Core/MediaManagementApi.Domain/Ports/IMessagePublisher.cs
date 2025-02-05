using MediaManagementApi.Domain.Entities;

namespace MediaManagementApi.Domain.Ports
{
    public interface IMessagePublisher
    {
        Task PublishVideoToProcessMessage(Video video, CancellationToken cancellationToken = default);
    }
}