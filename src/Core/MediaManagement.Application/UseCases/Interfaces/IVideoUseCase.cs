using MediaManagementApi.Domain.Entities;

namespace MediaManagement.Application.UseCases.Interfaces;

public interface IVideoUseCase
{
    Task<Video> ExecuteAsync(string emailUser, FileStream fileStream);
}