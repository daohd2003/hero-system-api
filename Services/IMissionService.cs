using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.DTOs.MissionDtos;

namespace Services
{
    public interface IMissionService
    {
        Task<ServiceResult<MissionDto>> CreateMissionAsync(CreateMissionDto dto);
        Task<ServiceResult<List<MissionDto>>> GetAllMissionsAsync();
        Task<ServiceResult<bool>> AssignMissionToHeroAsync(AssignMissionDto dto);
    }
}
