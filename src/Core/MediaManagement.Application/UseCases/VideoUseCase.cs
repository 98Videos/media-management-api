using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Repositories;

namespace MediaManagement.Application.UseCases;

public class VideoUseCase : IVideoUseCase
{
    private readonly IVideoRepository _videoRepository;

    public VideoUseCase(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    public async Task<Video> ExecuteAsync(string emailUser, Stream stream, string fileName)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream), "O stream não pode ser nulo.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("O nome do arquivo não pode ser nulo ou vazio.", nameof(fileName));
        }

        Video video = new Video(
            emailUser: emailUser,
            filename: fileName,
            VideoStatus.NAO_PROCESSADO);

        return await _videoRepository.AddAsync(video);
    }
}
