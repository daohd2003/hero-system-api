using BusinessObject.Models;
using DataAccess;

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

        public void Delete(Faction faction)
        {
            _context.Factions.Remove(faction);
        }

        public async Task<Faction?> GetByIdAsync(Guid id)
        {
            return await _context.Factions.FindAsync(id);
        }

        public void Update(Faction faction)
        {
            _context.Factions.Update(faction);
        }

        public IQueryable<Faction> GetQueryable()
        {
            return _context.Factions;
        }
    }
}
