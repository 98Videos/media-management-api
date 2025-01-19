namespace MediaManagementApi.Domain.Repositories;

public interface IVideoRepository
{
    Task<Video> AddAsync(Video video);
    Task<Video> GetVideoAsync(string videoId);
    Task<Video> UpdateAsync(Video video);
}