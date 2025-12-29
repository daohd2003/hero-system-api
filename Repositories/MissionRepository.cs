using BusinessObject.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<Mission?> GetByIdAsync(Guid id)
        {
            return await _context.Missions.FindAsync(id);
        }

        public async Task<List<Mission>> GetAllAsync()
        {
            return await _context.Missions.ToListAsync();
        }

        public async Task<HeroMission?> GetHeroMissionAsync(Guid heroId, Guid missionId)
        {
            return await _context.HeroMissions.FirstOrDefaultAsync(hm => hm.HeroId == heroId && hm.MissionId == missionId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
