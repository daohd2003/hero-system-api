using DataAccess;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IHeroRepository? _heroRepository;
        private IMissionRepository? _missionRepository;
        private IFactionRepository? _factionRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IHeroRepository Heroes => _heroRepository ??= new HeroRepository(_context);
        public IMissionRepository Missions => _missionRepository ??= new MissionRepository(_context);
        public IFactionRepository Factions => _factionRepository ??= new FactionRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
