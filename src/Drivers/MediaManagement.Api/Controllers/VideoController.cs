using MediaManagement.Api.Extensions;
using MediaManagement.Api.Services;
using MediaManagement.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly IVideoUseCase _videoUseCase;
    private readonly ICognitoUserInfoService _cognitoIdentityService;

    public VideoController(IVideoUseCase videoUseCase, ICognitoUserInfoService cognitoUserInfoService)
    {
        _videoUseCase = videoUseCase;
        _cognitoIdentityService = cognitoUserInfoService;
    }

    /// <summary>
    /// Realiza o upload de um vídeo para o sistema.
    /// </summary>
    /// <param name="file">Arquivo de vídeo a ser enviado.</param>
    /// <returns>Retorna informações do vídeo enviado ou mensagem de erro.</returns>
    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "O arquivo enviado é inválido ou está vazio." });
        }

        try
        {
            var userToken = Request.GetJwtBearerToken();
            var userInformation = await _cognitoIdentityService.GetUserInformationAsync(userToken);

            var video = await _videoUseCase.ExecuteAsync(
                emailUser: userInformation.Email,
                stream: file.OpenReadStream(),
                fileName: file.FileName
            );

            return Ok(new
            {
                message = "Upload realizado com sucesso.",
                videoId = video.Id,
                fileName = video.Filename,
                status = video.Status.ToString()
            });
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
    
    /// <summary>
    /// Atualiza o status de um vídeo.
    /// </summary>
    /// <param name="videoId">ID do vídeo a ser atualizado.</param>
    /// <returns>Retorna o vídeo atualizado ou mensagem de erro.</returns>
    [HttpPut("{videoId:guid}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateVideoStatus(Guid videoId)
    {
        try
        {
            var updatedVideo = await _videoUseCase.UpdateStatus(videoId);

            return Ok(new
            {
                message = "Status do vídeo atualizado com sucesso.",
                videoId = updatedVideo.Id,
                status = updatedVideo.Status.ToString()
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno no servidor.", details = ex.Message });
        }
    }
}
