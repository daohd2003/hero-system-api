using BusinessObject.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;

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

        public void UpdateAsync(Faction faction)
        {
            _context.Factions.Update(faction);
        }
    }
}
