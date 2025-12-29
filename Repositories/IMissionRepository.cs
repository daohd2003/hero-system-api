using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IMissionRepository
    {
        Task<Mission?> GetByIdAsync(Guid id);
        Task<List<Mission>> GetAllAsync();
        Task AddAsync(Mission mission);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
        Task AddHeroMissionAsync(HeroMission heroMission);
        Task<HeroMission?> GetHeroMissionAsync(Guid heroId, Guid missionId);
    }
}
