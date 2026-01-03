using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            // Controller CHỈ điều phối - không có logic validate
            var result = await _chatService.UploadImageAsync(file);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(new { url = result.Data });
        }
    }
}
