using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers.Controllers
{
    [Route("api/heroes")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly IHeroService _heroService;

        public HeroController(IHeroService heroService)
        {
            _heroService = heroService;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAllHeros([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
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
