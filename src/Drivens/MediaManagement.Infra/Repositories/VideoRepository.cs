using MediaManagement.Database.Data;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Repositories;

namespace MediaManagement.Database.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly VideoDbContext _dbContext;

    public VideoRepository(VideoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Video> AddAsync(Video video)
    {
        return await Task.FromResult(_dbContext.Video.Add(video).Entity);
    }

    public async Task<Video> GetVideoAsync(Guid videoId)
    {
        return await Task.FromResult(_dbContext.Video.Find(videoId)) ?? throw new InvalidOperationException();
    }

    public async Task<Video> UpdateAsync(Video video)
    {
        return await Task.FromResult(_dbContext.Video.Update(video).Entity);
    }

    public async Task<IEnumerable<Video>> GetAllVideosByUserAsync(string emailUser)
    {
        return await Task.FromResult(_dbContext.Video.Where(v => v.EmailUser == emailUser)) ?? throw new InvalidOperationException();
    }
}