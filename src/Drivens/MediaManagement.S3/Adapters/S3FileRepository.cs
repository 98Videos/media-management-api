using Amazon.S3;
using MediaManagement.S3.Options;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaManagement.S3.Adapters;

public class S3FileRepository : IFileRepository
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3BucketOptions _options;
    private readonly ILogger _logger;
    
    public S3FileRepository(IAmazonS3 amazonS3,
            IOptions<S3BucketOptions> options,
            ILogger<S3FileRepository> logger)
    {
        this._s3Client = amazonS3;
        this._logger = logger;
        this._options = options.Value;
    }
    
    public async Task<VideoFile> GetVideoFile(string userEmail, string fileIdentifier)
    {
        try
        {
            _logger.LogInformation($"getting video stream {userEmail}/{fileIdentifier} on s3...");

            var s3Response = await _s3Client.GetObjectAsync(_options.VideosBucket, $"{userEmail}/{fileIdentifier}");

            var videoFile = new VideoFile(fileIdentifier, s3Response.ResponseStream);
            return videoFile;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "could not retrieve object {fileIdentifier} for user {userEmail}", fileIdentifier, userEmail);
            throw;
        }
    }
}