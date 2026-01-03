using BusinessObject.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class MissionRepository : IMissionRepository
    {
        private readonly AppDbContext _context;

        public MissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Mission mission)
        {
            await _context.Missions.AddAsync(mission);
        }

        public async Task AddHeroMissionAsync(HeroMission heroMission)
        {
            await _context.HeroMissions.AddAsync(heroMission);
        }

        public async Task<Mission?> GetByIdAsync(Guid id)
        {
            return await _context.Missions.FindAsync(id);
        }

        public async Task<HeroMission?> GetHeroMissionAsync(Guid heroId, Guid missionId)
        {
            return await _context.HeroMissions.FirstOrDefaultAsync(hm => hm.HeroId == heroId && hm.MissionId == missionId);
        }

        public IQueryable<Mission> GetQueryable()
        {
            return _context.Missions;
        }
    }
}
