using MediaManagement.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly IVideoUseCase _videoUseCase;

    public VideoController(IVideoUseCase videoUseCase)
    {
        _videoUseCase = videoUseCase;
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Arquivo inválido ou vazio." });
        }

        try
        {
            string userEmail = "teste@teste.com";

            var video = await _videoUseCase.ExecuteAsync(
                emailUser: userEmail,
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
}