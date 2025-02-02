using MediaManagement.Database.Data;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MediaManagement.Database.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly VideoDbContext _dbContext;

    public VideoRepository(VideoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Video> AddAsync(Video video, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(video, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return video;
    }

    public async Task<Video?> GetVideoAsync(Guid videoId, CancellationToken cancellationToken = default)
    {
        var video = await _dbContext.Videos.AsNoTracking().SingleOrDefaultAsync(x => x.Id == videoId, cancellationToken);
        return video;
    }

    public async Task<Video> UpdateAsync(Video video, CancellationToken cancellationToken = default)
    {
        _dbContext.Videos.Update(video);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return video;
    }

    public async Task<IEnumerable<Video>> GetAllVideosByUserAsync(string emailUser, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Videos.AsQueryable().Where(x => x.EmailUser == emailUser).ToListAsync(cancellationToken: cancellationToken);
    }
}