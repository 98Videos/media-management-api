using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using MediaManagementApi.Domain.Entities;
using MediaManagement.Api.Controllers;
using MediaManagement.Application.UseCases.Interfaces;
using MediaManagement.Api.Models;
using Microsoft.AspNetCore.Http;
using MediaManagement.Api.Authentication;

namespace MediaManagement.UnitTests.Drivers.Api.Controllers;

[TestFixture]
public class ImageControllerTests
{
    private Mock<IImageUseCase> _imageUseCaseMock;
    private Mock<ICognitoUserInfoService> _cognitoUserInfoServiceMock;

    [SetUp]
    public void Setup()
    {
        _cognitoUserInfoServiceMock = new Mock<ICognitoUserInfoService>();
        _imageUseCaseMock = new Mock<IImageUseCase>();
    }

    [Test]
    public async Task DownloadZip_ShouldReturnFileResult()
    {
        var requestVideoId = Guid.NewGuid();
        var userInformation = new UserInformation("John Doe", "test@example.com");
        var zipFile = new ZipFile("test.zip", new MemoryStream(new byte[10]));

        // Configurar o mock do serviço Cognito para retornar o UserInformation
        _cognitoUserInfoServiceMock.Setup(s => s.GetUserInformationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInformation);

        //Mock
        _imageUseCaseMock.Setup(u => u.DownloadZipFileAsync(userInformation.Email, requestVideoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(zipFile);

        // Criar o HttpContext e mockar o Authorization header
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer valid.token";
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var controller = new ImagesController(_cognitoUserInfoServiceMock.Object, _imageUseCaseMock.Object)
        {
            ControllerContext = controllerContext // Definir o contexto no controller
        };

        var result = await controller.DownloadImages(requestVideoId, It.IsAny<CancellationToken>());

        Console.WriteLine($"Result Type: {result.GetType().Name}");

        if (result is ObjectResult objResult)
        {
            Console.WriteLine($"Status Code: {objResult.StatusCode}");
            Console.WriteLine($"Value: {objResult.Value}");
        }
        Assert.IsInstanceOf<FileStreamResult>(result);
    }
}