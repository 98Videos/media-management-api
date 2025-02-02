using MediaManagement.Api.Extensions;
using MediaManagement.Api.Services;
using MediaManagement.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ICognitoUserInfoService _cognitoIdentityService;
        private readonly IImageUseCase _imageUseCase;

        public ImageController(ICognitoUserInfoService cognitoIdentityService, IImageUseCase imageUseCase)
        {
            _cognitoIdentityService = cognitoIdentityService;
            _imageUseCase = imageUseCase;
        }

        [HttpGet("download")]
        [Authorize]
        public async Task<IActionResult> DownloadImages(string fileIdentifier)
        {
            if (string.IsNullOrWhiteSpace(fileIdentifier))
            {
                return BadRequest(new { message = "O arquivo enviado é inválido ou está vazio." });
            }
            try
            {
                var userToken = Request.GetJwtBearerToken();
                var userInformation = await _cognitoIdentityService.GetUserInformationAsync(userToken);
                await _imageUseCase.GetZipAsync(userInformation.Email, fileIdentifier);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor.", details = ex.Message });
            }
        }
    }
}
