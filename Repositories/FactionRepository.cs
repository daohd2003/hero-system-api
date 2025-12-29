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
    public class FactionRepository : IFactionRepository
    {
        private readonly AppDbContext _context;

        public FactionRepository(AppDbContext context)
        {

            _context = context;
        }

        public async Task AddAsync(Faction faction)
        {
            await _context.Factions.AddAsync(faction);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void DeleteAsync(Faction faction)
        {
            _context.Factions.Remove(faction);
        }

        public async Task<List<Faction>> GetAllAsync()
        {
            return await _context.Factions.Include(f => f.Heroes).AsNoTracking().ToListAsync();
        }

        public async Task<Faction?> GetByIdAsync(Guid id)
        {
            return await _context.Factions.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateAsync(Faction faction)
        {
            _context.Factions.Update(faction);
        }
    }
}
