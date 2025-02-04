using MediaManagement.Api.Contracts.Requests;
using MediaManagement.Api.Contracts.Responses;
using Moq;
using NUnit.Framework;
using MediaManagement.Api.Controllers;
using MediaManagement.Api.Services;
using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediaManagement.Api.Models;

namespace MediaManagement.UnitTests.Drivers.Api.Controllers;

[TestFixture]
public class VideosControllerTests
{
    private Mock<IVideoUseCase> _videoUseCaseMock;
    private Mock<ICognitoUserInfoService> _cognitoUserInfoServiceMock;
    private VideosController _controller;

    [SetUp]
    public void Setup()
    {
        _videoUseCaseMock = new Mock<IVideoUseCase>();
        _cognitoUserInfoServiceMock = new Mock<ICognitoUserInfoService>();
        _controller = new VideosController(_videoUseCaseMock.Object, _cognitoUserInfoServiceMock.Object);
    }

    [Test]
    public async Task UploadVideo_ShouldReturnBadRequest_WhenFileIsNull()
    {
        // Arrange
        IFormFile file = null;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.UploadVideo(file, cancellationToken);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task UploadVideo_ShouldReturnBadRequest_WhenFileIsEmpty()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.UploadVideo(fileMock.Object, cancellationToken);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task UploadVideo_ShouldReturnOk_WhenFileIsValid()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024); // Define o tamanho do arquivo mockado
        fileMock.Setup(f => f.FileName).Returns("video.mp4"); // Define o nome do arquivo mockado
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream()); // Retorna um MemoryStream para simular a leitura do arquivo

        var cancellationToken = CancellationToken.None;
        
        // Mock de UserInformation
        var userInformation = new UserInformation("John Doe", "test@example.com");

        // Mock do serviço Cognito para retornar as informações do usuário
        _cognitoUserInfoServiceMock.Setup(s => s.GetUserInformationAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(userInformation);

        // Mock do use case para retornar um vídeo com as informações esperadas
        var video = new Video("test@example.com", "video.mp4");

        _videoUseCaseMock.Setup(v => v.ExecuteAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(video);

        // Criar o HttpContext e mockar o Authorization header
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer valid.token";
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var controller = new VideosController(_videoUseCaseMock.Object, _cognitoUserInfoServiceMock.Object)
        {
            ControllerContext = controllerContext // Definir o contexto no controller
        };

        // Act
        var result = await controller.UploadVideo(fileMock.Object, cancellationToken);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");

        var response = okResult?.Value as VideoUploadResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("Upload realizado com sucesso.", response.Message);
        Assert.AreEqual(video.Id, response.VideoId);
        Assert.AreEqual(video.Filename, response.FileName);
        Assert.AreEqual(video.Status, response.Status);
    }

    [Test]
    public async Task UpdateVideoStatus_ShouldReturnBadRequest_WhenInvalidStatus()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var request = new UpdateVideoRequest { Status = (VideoStatus)999 };
        var cancellationToken = CancellationToken.None;

        _videoUseCaseMock.Setup(v => v.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<VideoStatus>(), cancellationToken))
            .ThrowsAsync(new ArgumentException("Status inválido"));

        // Act
        var result = await _controller.UpdateVideoStatus(videoId, request, cancellationToken);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task UpdateVideoStatus_ShouldReturnOk_WhenStatusUpdatedSuccessfully()
    {
        
        var request = new UpdateVideoRequest { Status = VideoStatus.Processado };
        var cancellationToken = CancellationToken.None;

        var video = new Video("test@example.com", "video.mp4");

        _videoUseCaseMock.Setup(v => v.UpdateStatusAsync(video.Id, request.Status, cancellationToken))
            .ReturnsAsync(video);

        // Act
        var result = await _controller.UpdateVideoStatus(video.Id, request, cancellationToken);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult.Value as UpdateVideoResponse;
        Assert.AreEqual(video.Id, response.VideoId);
        Assert.AreEqual(video.Status, response.Status);
        Assert.AreEqual("Status do vídeo atualizado com sucesso.", response.Message);
    }

    [Test]
    public async Task GetVideosStatusList_ShouldReturnOk_WhenVideosFound()
    {
        // Arrange
        var userInformation = new UserInformation("John Doe", "test@example.com");
        var cancellationToken = CancellationToken.None;

        var videoList = new List<Video>
        {
            new Video("test@example.com", "video1.mp4"),
            new Video("test@example.com", "video2.mp4")
        };

        // Configurar o mock do serviço Cognito para retornar o UserInformation
        _cognitoUserInfoServiceMock.Setup(s => s.GetUserInformationAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(userInformation);

        // Configurar o mock do use case para retornar a lista de vídeos
        _videoUseCaseMock.Setup(v => v.GetAllVideosByUserAsync(userInformation.Email, cancellationToken))
            .ReturnsAsync(videoList);

        // Criar o HttpContext e mockar o Authorization header
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer valid.token";
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var controller = new VideosController(_videoUseCaseMock.Object, _cognitoUserInfoServiceMock.Object)
        {
            ControllerContext = controllerContext // Definir o contexto no controller
        };

        // Act
        var result = await controller.GetVideosStatusList(cancellationToken);

        // Assert
        Assert.IsNotNull(result, "Expected OkObjectResult, but got null.");

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");

        var response = okResult?.Value as IEnumerable<Video>;
        Assert.IsNotNull(response, "Expected non-null video list, but got null.");
        Assert.That(response?.Count(), Is.EqualTo(2), "The number of videos returned is incorrect.");
    }
}
