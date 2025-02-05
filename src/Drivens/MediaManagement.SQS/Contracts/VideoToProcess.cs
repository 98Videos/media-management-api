using MassTransit;

namespace MediaManagement.SQS.Contracts
{
    [MessageUrn("video-to-process-message")]
    public class VideoToProcessMessage
    {
        public required string VideoId { get; set; }
        public required string UserEmail { get; set; }
    }
}
