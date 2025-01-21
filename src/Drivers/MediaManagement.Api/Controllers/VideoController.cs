using MediaManagementApi.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MediaManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController: ControllerBase
{
    private readonly IVideoRepository _videoRepository;

    public VideoController(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadVideo([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) 
            return BadRequest();
        
    }
}