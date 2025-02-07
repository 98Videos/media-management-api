using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Repositories;

namespace MediaManagement.Application.UseCases;

public class ImageUseCase : IImageUseCase
{
    private readonly IFileRepository _fileRepository;
    private readonly IVideoRepository _videoRepository;

    public ImageUseCase(IFileRepository fileRepository, IVideoRepository videoRepository)
    {
        _fileRepository = fileRepository;
        _videoRepository = videoRepository;
    }

    public async Task<ZipFile?> DownloadZipFileAsync(string userEmail, Guid videoIdentifier, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(userEmail)) 
        {  
            throw new ArgumentNullException(nameof(userEmail));
        }

        var video = await _videoRepository.GetVideoAsync(videoIdentifier, cancellationToken);
        if (video == null || video.Status != VideoStatus.Processado)
            return null;

        var zipFileName = $"{videoIdentifier}_thumbs.zip";
        var downloadedFile = await _fileRepository.GetZipFileAsync(userEmail, zipFileName, cancellationToken);

        return new ZipFile($"{video.Filename}.zip", downloadedFile.FileStreamReference);
    }
}
