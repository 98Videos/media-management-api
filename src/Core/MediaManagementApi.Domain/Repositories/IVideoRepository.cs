using MediaManagementApi.Domain.Entities;

namespace MediaManagementApi.Domain.Repositories;

public interface IVideoRepository
{
    Task<Video> AddAsync(Video video, CancellationToken cancellationToken = default);
    Task<Video?> GetVideoAsync(Guid videoId, CancellationToken cancellationToken = default);
    Task<Video> UpdateAsync(Video video, CancellationToken cancellationToken = default);
    Task<IEnumerable<Video>> GetAllVideosByUserAsync(string emailUser, CancellationToken cancellationToken = default);
}