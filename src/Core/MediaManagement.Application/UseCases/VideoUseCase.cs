using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Repositories;

namespace MediaManagement.Application.UseCases;

public class VideoUseCase: IVideoUseCase
{   
    private readonly IVideoRepository _videoRepository;

    public async Task<Video> ExecuteAsync(string emailUser, FileStream fileStream)
    {
        Video video = new Video(
            emailUser: emailUser,
            filename: fileStream.Name,
            VideoStatus.NAO_PROCESSADO);

        return await _videoRepository.AddAsync(video);
    }
}