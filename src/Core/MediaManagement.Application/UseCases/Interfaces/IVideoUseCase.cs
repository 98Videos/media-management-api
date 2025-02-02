using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;

namespace MediaManagement.Application.UseCases.Interfaces;

public interface IVideoUseCase
{ 
    Task<Video> ExecuteAsync(string emailUser, Stream stream, string fileName, CancellationToken cancellationToken = default);
    Task<Video> UpdateStatusAsync(Guid videoId, VideoStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Video>> GetAllVideosByUserAsync(string emailUser, CancellationToken cancellationToken = default);
}