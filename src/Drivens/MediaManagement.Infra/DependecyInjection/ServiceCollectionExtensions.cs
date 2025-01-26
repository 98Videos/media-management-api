using MediaManagement.Database.Data;
using MediaManagement.Database.Repositories;
using MediaManagementApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManagement.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<VideoDbContext>(
            opt => opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        bool.TryParse(configuration.GetSection("RunMigrationsOnStartup").Value, out var shouldRunMigrations);

        if (shouldRunMigrations)
            services.MigrateDatabase();

        services.AddScoped<IVideoRepository, VideoRepository>();
        
        return services;
    }

    private static void MigrateDatabase(this IServiceCollection services)
    {
        using var serviceScope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetService<VideoDbContext>() ??
                      throw new ApplicationException("Failed to migrate database!");

        context.Database.Migrate();
    }
}