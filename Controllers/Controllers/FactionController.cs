using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using static BusinessObject.DTOs.FactionDtos;

namespace Controllers.Controllers
{
    [Route("api/factions")]
    [ApiController]
    public class FactionController : ControllerBase
    {
        private readonly IFactionService _factionService;

        public FactionController(IFactionService factionService)
        {
            _factionService = factionService;
        }

        // 1. GET: api/factions
        // Lấy danh sách tất cả Faction
        [HttpGet]
        public async Task<IActionResult> GetAllFactions()
        {
            var result = await _factionService.GetAllFactionsAsync();

            // Logic trả về chung: Dựa vào StatusCode từ ServiceResult
            if (!result.Success)
            {
                // Trả về Message lỗi nếu thất bại (400/404/500)
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            // Trả về Data nếu thành công (200)
            return StatusCode(result.StatusCode, result.Data);
        }

        // 2. POST: api/factions
        // Tạo mới Faction
        [HttpPost]
        public async Task<IActionResult> CreateFaction([FromBody] CreateFactionDto dto)
        {
            var result = await _factionService.CreateFactionAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            // Thường Create thành công trả về 201 Created
            return StatusCode(result.StatusCode, result.Data);
        }

        // 3. DELETE: api/factions/{id}
        // Xóa Faction theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaction(Guid id)
        {
            var result = await _factionService.DeleteFactionAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            // Delete thành công thường trả về 200 OK hoặc 204 No Content
            return StatusCode(result.StatusCode, new { Message = result.Message });
        }
    }
}
