using BusinessObject.DTOs;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.DTOs.FactionDtos;

namespace Services
{
    public interface IFactionService
    {
        Task<ServiceResult<FactionDto>> CreateFactionAsync(CreateFactionDto dto);
        Task<ServiceResult<bool>> DeleteFactionAsync(Guid id);
        Task<ServiceResult<List<FactionDtos.FactionDto>>> GetAllFactionsAsync();
    }
}
