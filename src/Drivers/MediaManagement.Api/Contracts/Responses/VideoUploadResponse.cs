using System.Text.Json.Serialization;
using MediaManagementApi.Domain.Enums;

namespace MediaManagement.Api.Contracts.Responses
{
    public record VideoUploadResponse
    {
        public required string FileName { get; set; }
        public required string Message { get; set; }
        public required Guid VideoId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required VideoStatus Status { get; set; }
    }
}