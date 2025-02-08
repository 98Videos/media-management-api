using Moq;
using NUnit.Framework;
using MediaManagementApi.Domain.Entities;
using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Repositories;
using MediaManagement.Application.UseCases;
using AutoBogus;
using MediaManagementApi.Domain.Enums;

namespace MediaManagement.UnitTests.Core.Application.UseCases;

[TestFixture]
public class ImageUseCaseTests
{
    private Mock<IFileRepository> _fileRepositoryMock;
    private Mock<IVideoRepository> _videoRepositoryMock;
    private IImageUseCase _imageUseCase;

    [SetUp]
    public void Setup()
    {
        _fileRepositoryMock = new Mock<IFileRepository>();
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _imageUseCase = new ImageUseCase(_fileRepositoryMock.Object, _videoRepositoryMock.Object);
    }

    [Test]
    public async Task GetZipFileAsync_WhenVideoExistsAndIsProcessed_ShouldReturnZipFile()
    {
        var existingVideo = new AutoFaker<Video>()
            .RuleFor(x => x.Status, VideoStatus.Processado)
            .Generate();

        var zipFileNameForVideo = $"{existingVideo.Id}_thumbs.zip";
        var zipFile = new ZipFile("test.zip", new MemoryStream());

        _videoRepositoryMock.Setup(x => x.GetVideoAsync(existingVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingVideo);

        _fileRepositoryMock.Setup(r => r.GetZipFileAsync("user@example.com", zipFileNameForVideo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(zipFile);

        var result = await _imageUseCase.DownloadZipFileAsync("user@example.com", existingVideo.Id);

        Assert.That(result!.FileName, Is.EqualTo($"{existingVideo.Filename}.zip"));
        Assert.That(result!.FileStreamReference, Is.EqualTo(zipFile.FileStreamReference));
    }

    [Test]
    public async Task GetZipFileAsync_WhenVideoExistsButIsNotBeenProcessedYet_ShouldReturnNull()
    {
        var existingVideo = new AutoFaker<Video>()
            .RuleFor(x => x.Status, VideoStatus.EmProcessamento)
            .Generate();

        _videoRepositoryMock.Setup(x => x.GetVideoAsync(existingVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingVideo);

        var result = await _imageUseCase.DownloadZipFileAsync("user@example.com", existingVideo.Id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetZipFileAsync_WhenVideoDoesNotExist_ShouldReturnNull()
    {
        var requestedVideoId = Guid.NewGuid();

        _videoRepositoryMock.Setup(x => x.GetVideoAsync(requestedVideoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Video?)null);

        var result = await _imageUseCase.DownloadZipFileAsync("user@example.com", requestedVideoId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetZipFileAsync_WhenVideoDoesNotExist_ShouldNotTryToDownloadFile()
    {
        var requestedVideoId = Guid.NewGuid();

        _videoRepositoryMock.Setup(x => x.GetVideoAsync(requestedVideoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Video?)null);

        var result = await _imageUseCase.DownloadZipFileAsync("user@example.com", requestedVideoId);

        _fileRepositoryMock.Verify(x => 
            x.GetVideoFileAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), 
            Times.Never()); 
    }
}
