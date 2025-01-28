using MediaManagementApi.Domain.Entities;

namespace MediaManagement.Application.UseCases.Interfaces;

public interface IVideoUseCase
{ 
    Task<Video> ExecuteAsync(string emailUser, Stream stream, string fileName);
    Task<Video> UpdateStatus(Guid videoId);
}