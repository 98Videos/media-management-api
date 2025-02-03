using MediaManagement.Api.Contracts.Requests;
using MediaManagement.Api.Contracts.Responses;
using MediaManagement.Api.Extensions;
using MediaManagement.Api.Services;
using MediaManagement.Application.UseCases.Interfaces;
using MediaManagementApi.Domain.Entities;
using MediaManagementApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
{
    private readonly IVideoUseCase _videoUseCase;
    private readonly ICognitoUserInfoService _cognitoIdentityService;

    public VideosController(IVideoUseCase videoUseCase, ICognitoUserInfoService cognitoUserInfoService)
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
    public async Task<IActionResult> UploadVideo(IFormFile file, CancellationToken cancellationToken)
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
                fileName: file.FileName,
                cancellationToken: cancellationToken
            );

            return Ok(new VideoUploadResponse()
            {
                Message = "Upload realizado com sucesso.",
                VideoId = video.Id,
                FileName = video.Filename,
                Status = video.Status
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
    public async Task<IActionResult> UpdateVideoStatus(Guid videoId, UpdateVideoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updatedVideo = await _videoUseCase.UpdateStatusAsync(videoId, request.Status, cancellationToken);

            return Ok(new UpdateVideoResponse()
            {
                VideoId = updatedVideo.Id,
                Status = updatedVideo.Status,
                Message = "Status do vídeo atualizado com sucesso."
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

    /// <summary>
    /// Recupera os status dos vídeos enviados pelo usuário.
    /// </summary>
    /// <returns>Lista de vídeos com os respectivos status.</returns>
    [HttpGet("videolist")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<Video>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVideosStatusList(CancellationToken cancellationToken)
    {
        try
        {
            var userToken = Request.GetJwtBearerToken();
            var userInformation = await _cognitoIdentityService.GetUserInformationAsync(userToken, cancellationToken);
            var videoList = await _videoUseCase.GetAllVideosByUserAsync(userInformation.Email, cancellationToken);
            return Ok(videoList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno no servidor.", details = ex.Message });
        }
    }
}