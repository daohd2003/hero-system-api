using BusinessObject.DTOs.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHeroAuthService _authService;

        public AuthController(IHeroAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] HeroLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Success) return StatusCode(result.StatusCode, result.Message);
            return Ok(result.Data);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            if (!result.Success) return StatusCode(result.StatusCode, result.Message);
            return Ok(result.Data);
        }
    }
}
