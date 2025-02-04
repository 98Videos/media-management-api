using MassTransit;

namespace MediaManagement.SQS.Contracts
{
    [MessageUrn("video-to-process-message")]
    public class VideoToProcessMessage
    {
        public string VideoId { get; set; }
        public string UserEmail { get; set; }
    }
}
