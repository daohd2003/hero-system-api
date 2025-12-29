using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IFactionRepository
    {
        Task AddAsync(Faction faction);
        void UpdateAsync(Faction faction);
        void DeleteAsync(Faction faction);
        Task<List<Faction>> GetAllAsync();
        Task<Faction?> GetByIdAsync(Guid id);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
    }
}
