using MediaManagementApi.Domain.Entities;

namespace MediaManagementApi.Domain.Repositories;

public interface IFileRepository
{
    Task<VideoFile> GetVideoFile(string userEmail, string fileIdentifier);
    Task UploadVideoFile(string userEmail, string fileIdentifier, Stream videoStream);
}