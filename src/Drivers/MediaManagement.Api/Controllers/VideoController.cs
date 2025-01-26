using MediaManagement.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediaManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController: ControllerBase
{
    private readonly IVideoUseCase _videoUseCase;

    public VideoController(IVideoUseCase _videoUseCase)
    {
        this._videoUseCase = _videoUseCase;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadVideo([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) 
            return BadRequest();
        
        this._videoUseCase.ExecuteAsync("teste@teste.com", file.OpenReadStream(), file.FileName).Wait();
        
        return Ok();
    }
}