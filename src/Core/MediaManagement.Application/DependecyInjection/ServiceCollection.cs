using MediaManagement.Application.UseCases;
using MediaManagement.Application.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManagement.Application.DependecyInjection;

public static class ServiceCollection 
{
    public static IServiceCollection AddVideoUseCase(this IServiceCollection services)
    {
        services.AddScoped<IVideoUseCase, VideoUseCase>();
        return services;
    }
}