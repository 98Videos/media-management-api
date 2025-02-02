using MediaManagementApi.Domain.Entities;

namespace MediaManagement.Application.UseCases.Interfaces;

public interface IImageUseCase
{
    Task<ZipFile> GetZipAsync(string userEmail, string identifier);
}
