using BusinessObject.DTOs;
using BusinessObject.Models;
using Services.Common;

namespace Services
{
    public interface IHeroService
    {
        Task<ServiceResult<Hero>> CreateHeroAsync(CreateHeroDto dto);
        Task<ServiceResult<HeroDto?>> GetHeroByIdAsync(Guid id);
        Task<ServiceResult<PagedResult<HeroDto>>> GetAllHerosAsync(int pageNumber, int pageSize);
        Task<ServiceResult<bool>> UpdateHeroAsync(Guid id, UpdateHeroDto dto);
        Task<ServiceResult<bool>> DeleteHeroAsync(Guid id);
        Task<ServiceResult<HeroDto?>> GetHeroFullInfoAsync(Guid id);
    }
}
