using BusinessObject.Models;

namespace Repositories
{
    public interface IMissionRepository
    {
        Task<Mission?> GetByIdAsync(Guid id);
        Task<List<Mission>> GetAllAsync();
        Task AddAsync(Mission mission);
        Task AddHeroMissionAsync(HeroMission heroMission);
        Task<HeroMission?> GetHeroMissionAsync(Guid heroId, Guid missionId);
    }
}
