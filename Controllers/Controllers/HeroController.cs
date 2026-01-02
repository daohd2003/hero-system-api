using BusinessObject.DTOs;
using Controllers.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;

namespace Controllers.Controllers
{
    [Route("api/heroes")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly IHeroService _heroService;
        private readonly INotificationService _notificationService;

        public HeroController(IHeroService heroService, INotificationService notificationService)
        {
            _heroService = heroService;
            _notificationService = notificationService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateHeroDto dto)
        {
            var result = await _heroService.CreateHeroAsync(dto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Message);
            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetHeroById(Guid id)
        {
            var result = await _heroService.GetHeroByIdAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Message);
            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAllHeros([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var heroId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var heroName = User.Identity?.Name ?? "Ai đó";

            // Gửi thông báo
            await _notificationService.SendSystemNotificationAsync(heroId, $"{heroName} đã truy cập toàn bộ thông tin anh hùng.");

            var result = await _heroService.GetAllHerosAsync(pageNumber, pageSize);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Message);
            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _heroService.DeleteHeroAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Message);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHeroDto dto)
        {
            var result = await _heroService.UpdateHeroAsync(id, dto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Message);
            return StatusCode(result.StatusCode, result.Data);
        }
    }
}
