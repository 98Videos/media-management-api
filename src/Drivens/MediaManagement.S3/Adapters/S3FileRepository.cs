using Amazon.S3;
using Amazon.S3.Model;
using MediaManagement.S3.Exceptions;
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
        if (string.IsNullOrEmpty(userEmail)) 
            throw new ArgumentException("User email cannot be null or empty", nameof(userEmail));
        if (string.IsNullOrEmpty(fileIdentifier)) 
            throw new ArgumentException("File identifier cannot be null or empty", nameof(fileIdentifier));

        try
        {
            _logger.LogInformation("Fetching file from bucket '{Bucket}' at key '{Key}'", _options.VideosBucket, $"{userEmail}/{fileIdentifier}");

            var s3Response = await _s3Client.GetObjectAsync(_options.VideosBucket, $"{userEmail}/{fileIdentifier}");

            // Certifique-se de que o VideoFile manipula o stream corretamente
            var videoFile = new VideoFile(fileIdentifier, s3Response.ResponseStream);
            return videoFile;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching object '{FileIdentifier}' for user '{UserEmail}' from bucket '{Bucket}'", fileIdentifier, userEmail, _options.VideosBucket);
            throw new FileRetrievalException($"Error fetching file {fileIdentifier} for user {userEmail}", e);
        }
    }

    public async Task<ZipFile> GetZipFile(string userEmail, string fileIdentifier)
    {
        if (string.IsNullOrEmpty(userEmail))
            throw new ArgumentException("User email cannot be null or empty", nameof(userEmail));
        if (string.IsNullOrEmpty(fileIdentifier))
            throw new ArgumentException("File identifier cannot be null or empty", nameof(fileIdentifier));

        try
        {
            var key = $"{userEmail}/{fileIdentifier}";
            _logger.LogInformation("Downloading zip file from bucket '{Bucket}' at key '{Key}'", _options.VideosBucket, key);

            var request = new GetObjectRequest
            {
                BucketName = _options.VideosBucket,
                Key = key,
            };

            using var response = await _s3Client.GetObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                using var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var tempFilePath = Path.GetTempFileName();
                await File.WriteAllBytesAsync(tempFilePath, memoryStream.ToArray());

                _logger.LogInformation("Successfully downloaded zip file from S3: {Key}", key);
                return new ZipFile(tempFilePath);
            }
            else
            {
                _logger.LogWarning("Download from S3 completed but returned status: {StatusCode}", response.HttpStatusCode);
                throw new FileDownloadException("Failed to retrieve file from S3");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error downloading file '{FileIdentifier}' for user '{UserEmail}' from bucket '{Bucket}'", fileIdentifier, userEmail, _options.VideosBucket);
            throw new FileDownloadException($"Error downloading file {fileIdentifier} for user {userEmail}", e);
        }
    }


    public async Task UploadVideoFile(string userEmail, string fileIdentifier, Stream videoStream)
    {
        if (string.IsNullOrEmpty(userEmail)) 
            throw new ArgumentException("User email cannot be null or empty", nameof(userEmail));
        if (string.IsNullOrEmpty(fileIdentifier)) 
            throw new ArgumentException("File identifier cannot be null or empty", nameof(fileIdentifier));
        if (videoStream == null || videoStream.Length == 0) 
            throw new ArgumentException("Video stream cannot be null or empty", nameof(videoStream));
        try
        {
            var key = $"{userEmail}/{fileIdentifier}";

            _logger.LogInformation("Uploading video file to bucket '{Bucket}' at key '{Key}'", _options.VideosBucket, key);

            var request = new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = _options.VideosBucket,
                Key = key,
                InputStream = videoStream,
                AutoCloseStream = true,
            };

            var response = await _s3Client.PutObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully uploaded video file to S3: {Key}", key);
            }
            else
            {
                _logger.LogWarning("Upload to S3 completed but returned status: {StatusCode}", response.HttpStatusCode);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error uploading video file '{FileIdentifier}' for user '{UserEmail}' to bucket '{Bucket}'", fileIdentifier, userEmail, _options.VideosBucket);
            throw new FileUploadException($"Error uploading file {fileIdentifier} for user {userEmail}", e);
        }
    }
}