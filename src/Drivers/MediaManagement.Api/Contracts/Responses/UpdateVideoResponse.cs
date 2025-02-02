using MediaManagementApi.Domain.Enums;
using System.Text.Json.Serialization;

namespace MediaManagement.Api.Contracts.Responses
{
    public record UpdateVideoResponse
    {
        public required string Message { get; set; }
        public required Guid VideoId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required VideoStatus Status { get; set; }
    }
}