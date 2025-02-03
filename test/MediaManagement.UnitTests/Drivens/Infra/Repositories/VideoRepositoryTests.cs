using MediaManagement.Database.Data;
using MediaManagement.Database.Repositories;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MediaManagement.UnitTests.Drivens.Repositories;

[TestFixture]
public class VideoRepositoryTests
{
    private VideoRepository _repository;
    private DbContextOptions<VideoDbContext> _dbContextOptions;

    [SetUp]
    public void SetUp()
    {
        // Configura o banco de dados InMemory
        _dbContextOptions = new DbContextOptionsBuilder<VideoDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        var dbContext = new VideoDbContext(_dbContextOptions);
        _repository = new VideoRepository(dbContext);
    }

    [Test]
    public void Constructor_ShouldSetStatusToEmProcessamento_WhenVideoIsCreated()
    {
        // Act
        var video = new Video("user@example.com", "filename");

        // Assert
        Assert.AreEqual(VideoStatus.EmProcessamento, video.Status);
    }

    [Test]
    public async Task AddAsync_ShouldAddVideo()
    {
        // Arrange
        var video = new Video("testuser@example.com", "TestVideo.mp4");

        // Act
        var result = await _repository.AddAsync(video);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(video.Id, result.Id);
        Assert.AreEqual(video.Filename, result.Filename);
        Assert.AreEqual(VideoStatus.EmProcessamento, result.Status); // Verifica o status
    }

    [Test]
    public async Task GetVideoAsync_ShouldReturnVideo_WhenVideoExists()
    {
        // Arrange
        var video = new Video("testuser@example.com", "TestVideo.mp4");

        // Adiciona o vídeo ao contexto
        var dbContext = new VideoDbContext(_dbContextOptions);
        await dbContext.Videos.AddAsync(video);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetVideoAsync(video.Id);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(video.Id, result?.Id);
        Assert.AreEqual(video.Filename, result?.Filename);
        Assert.AreEqual(VideoStatus.EmProcessamento, result?.Status); // Verifica o status
    }

    [Test]
    public async Task GetVideoAsync_ShouldReturnNull_WhenVideoDoesNotExist()
    {
        // Act
        var result = await _repository.GetVideoAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Test]
    public async Task UpdateStatus_ShouldChangeStatus_WhenCalled()
    {
        // Arrange
        var video = new Video("testuser@example.com", "TestVideo.mp4");

        // Adiciona o vídeo ao contexto
        var dbContext = new VideoDbContext(_dbContextOptions);
        await dbContext.Videos.AddAsync(video);
        await dbContext.SaveChangesAsync();

        // Act
        video.UpdateStatus(VideoStatus.Processado);

        // Assert
        var updatedVideo = await dbContext.Videos.FindAsync(video.Id);
        Assert.AreEqual(VideoStatus.Processado, updatedVideo?.Status);
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateVideo_WhenVideoExists()
    {
        // Arrange
        var video = new Video("testuser@example.com", "TestVideo.mp4");

        // Adiciona o vídeo ao contexto
        var dbContext = new VideoDbContext(_dbContextOptions);
        await dbContext.Videos.AddAsync(video);
        await dbContext.SaveChangesAsync();

        // Atualiza os dados
        video.Filename = "UpdatedVideo.mp4";

        // Act
        var result = await _repository.UpdateAsync(video);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual("UpdatedVideo.mp4", result?.Filename);
    }

    [Test]
    public async Task GetAllVideosByUserAsync_ShouldReturnVideos_ForSpecificUser()
    {
        // Arrange
        var video1 = new Video("user@example.com", "Video1.mp4");
        var video2 = new Video("user@example.com", "Video2.mp4");
        var video3 = new Video("anotheruser@example.com", "Video3.mp4");

        // Adiciona os vídeos ao contexto
        var dbContext = new VideoDbContext(_dbContextOptions);
        await dbContext.Videos.AddAsync(video1);
        await dbContext.Videos.AddAsync(video2);
        await dbContext.Videos.AddAsync(video3);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllVideosByUserAsync("user@example.com");

        // Assert
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(v => v.Filename == "Video1.mp4"));
        Assert.IsTrue(result.Any(v => v.Filename == "Video2.mp4"));

    }
}