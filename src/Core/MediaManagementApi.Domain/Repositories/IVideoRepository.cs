using MediaManagementApi.Domain.Entities;

namespace MediaManagementApi.Domain.Repositories;

public interface IVideoRepository
{
    Task<Video> AddAsync(Video video);
    Task<Video> GetVideoAsync(Guid videoId);
    Task<Video> UpdateAsync(Video video);
}