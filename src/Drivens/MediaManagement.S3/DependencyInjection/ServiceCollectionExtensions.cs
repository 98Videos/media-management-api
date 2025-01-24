using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using MediaManagement.S3.Adapters;
using MediaManagement.S3.Options;
using MediaManagementApi.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManagement.S3.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS3FileManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3BucketOptions>(configuration.GetSection(nameof(S3BucketOptions)));

        var s3Client = new AmazonS3Client(new EnvironmentVariablesAWSCredentials(), RegionEndpoint.USEast1);

        services.AddSingleton<IAmazonS3>(s3Client);
        services.AddScoped<IFileRepository, S3FileRepository>();

        return services;
    }
}