using MediaManagementApi.Domain.Enums;

namespace MediaManagement.Api.Contracts.Requests
{
    public record UpdateVideoRequest
    {
        public VideoStatus Status { get; set; }
    }
}