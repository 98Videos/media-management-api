using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Repositories;

namespace MediaManagement.Application.UseCases;

public class ImageUseCase : IImageUseCase
{
    private readonly IFileRepository _fileRepository;

    public ImageUseCase(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<ZipFile> GetZipAsync(string userEmail, string identifier)
    {
        if(string.IsNullOrWhiteSpace(userEmail)) 
        {  
            throw new ArgumentNullException(nameof(userEmail));
        }
        if(string.IsNullOrWhiteSpace(identifier)) 
        { 
            throw new ArgumentNullException(nameof(identifier)); 
        }
        return await _fileRepository.GetZipFile(userEmail, identifier);
    }
}
