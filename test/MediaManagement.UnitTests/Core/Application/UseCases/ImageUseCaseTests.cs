using Moq;
using NUnit.Framework;
using MediaManagementApi.Domain.Entities;
using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Repositories;
using MediaManagement.Application.UseCases;

namespace MediaManagement.UnitTests.Core.Application.UseCases;

[TestFixture]
public class ImageUseCaseTests
{
    private Mock<IFileRepository> _fileRepositoryMock;
    private IImageUseCase _imageUseCase;

    [SetUp]
    public void Setup()
    {
        _fileRepositoryMock = new Mock<IFileRepository>();
        _imageUseCase = new ImageUseCase(_fileRepositoryMock.Object);
    }

    [Test]
    public async Task GetZipFileAsync_ShouldReturnZipFile()
    {
        var zipFile = new ZipFile("test.zip", new MemoryStream());
        _fileRepositoryMock.Setup(r => r.GetZipFileAsync("user@example.com", "test.zip", It.IsAny<CancellationToken>()))
            .ReturnsAsync(zipFile);

        var result = await _imageUseCase.DownloadZipFileAsync("user@example.com", "test.zip");

        Assert.AreEqual(zipFile, result);
    }
}
