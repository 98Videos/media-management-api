using MediaManagementApi.Domain.Entities;

namespace MediaManagement.Application.UseCases.Interfaces;

public interface IImageUseCase
{
    Task<ZipFile?> DownloadZipFileAsync(string userEmail, Guid videoId, CancellationToken cancellationToken = default);
}
