namespace MediaManagement.Application.MessageContracts
{
    public record VideoToProcessMessage
    {
        public required string VideoId { get; init; }
        public required string UserEmail { get; init; }
    }
}
