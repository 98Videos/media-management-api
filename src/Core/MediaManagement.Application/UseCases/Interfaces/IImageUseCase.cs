using MediaManagementApi.Domain.Entities;

namespace MediaManagement.Application.UseCases.Interfaces;

public interface IImageUseCase
{
    Task<ZipFile> DownloadZipFileAsync(string userEmail, string identifier, CancellationToken cancellationToken = default);
}
