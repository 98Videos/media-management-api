using MediaManagementApi.Domain.Entities;

namespace MediaManagementApi.Domain.Repositories;

public interface IFileRepository
{
    Task<VideoFile> GetVideoFile(string userEmail, string fileIdentifier);
}