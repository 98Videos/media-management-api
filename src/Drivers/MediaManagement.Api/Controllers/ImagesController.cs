using MediaManagement.Api.Authentication;
using MediaManagement.Api.Extensions;
using MediaManagement.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ICognitoUserInfoService _cognitoIdentityService;
        private readonly IImageUseCase _imageUseCase;


        public ImagesController(ICognitoUserInfoService cognitoIdentityService, IImageUseCase imageUseCase)
        {
            _cognitoIdentityService = cognitoIdentityService;
            _imageUseCase = imageUseCase;
        }

        /// <summary>
        /// Faz o download do arquivo especificado para o usuário autenticado.
        /// </summary>
        /// <param name="videoId">Id do video enviado para gerar as imagens a serem baixadas</param>
        /// <returns>Retorna arquivo ZIP com imagens extraídas do vídeo.</returns>
        [HttpGet("download")]
        [Authorize(AuthenticationSchemes = AuthSchemes.BearerToken)]
        public async Task<IActionResult> DownloadImages(Guid videoId, CancellationToken cancellationToken)
        {
            if (videoId == Guid.Empty)
            {
                return BadRequest(new { message = "Id do video é inválido" });
            }
            try
            {
                var userToken = Request.GetJwtBearerToken();
                var userInformation = await _cognitoIdentityService.GetUserInformationAsync(userToken, cancellationToken);
                var zipFile = await _imageUseCase.DownloadZipFileAsync(userInformation.Email, videoId, cancellationToken);
                
                if (zipFile == null)
                {
                    return NotFound(new { message = $"Não foi encontrado um arquivo para o video ou ele não terminou de ser processado" });
                }
                
                return File(zipFile.FileStreamReference, "application/zip", zipFile.FileName);
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
