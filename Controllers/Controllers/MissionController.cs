using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using static BusinessObject.DTOs.MissionDtos;

namespace Controllers.Controllers
{
    [Route("api/missions")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly IMissionService _missionService;

        public MissionController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        // POST: api/missions
        [HttpPost]
        public async Task<IActionResult> CreateMission([FromBody] CreateMissionDto dto)
        {
            var result = await _missionService.CreateMissionAsync(dto);
            return StatusCode(result.StatusCode, result.Data);
        }

        // GET: api/missions
        [HttpGet]
        public async Task<IActionResult> GetAllMissions()
        {
            var result = await _missionService.GetAllMissionsAsync();
            return StatusCode(result.StatusCode, result.Data);
        }

        // POST: api/missions/assign
        [HttpPost("assign")]
        public async Task<IActionResult> AssignMission([FromBody] AssignMissionDto dto)
        {
            // Gọi Service
            var result = await _missionService.AssignMissionToHeroAsync(dto);

            if (!result.Success)
            {
                return StatusCode(500, result.Message);
            }
            return StatusCode(result.StatusCode, result.Data);
        }
    }
}
