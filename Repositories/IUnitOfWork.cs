using Microsoft.EntityFrameworkCore.Storage;

namespace Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IHeroRepository Heroes { get; }
        IMissionRepository Missions { get; }
        IFactionRepository Factions { get; }

        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
