using MediaManagementApi.Domain.Entities;
namespace MediaManagementApi.Domain.Repositories;

public interface IFileRepository
{
    Task<VideoFile> GetVideoFileAsync(string userEmail, string fileIdentifier, CancellationToken cancellationToken = default);
    Task UploadVideoFileAsync(string userEmail, string fileIdentifier, Stream videoStream, CancellationToken cancellationToken = default);
    Task<ZipFile> GetZipFileAsync(string userEmail, string fileIdentifier, CancellationToken cancellationToken = default);
}