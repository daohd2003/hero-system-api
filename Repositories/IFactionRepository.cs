using BusinessObject.Models;

namespace Repositories
{
    public interface IFactionRepository
    {
        Task AddAsync(Faction faction);
        void Update(Faction faction);
        void Delete(Faction faction);
        Task<Faction?> GetByIdAsync(Guid id);
        IQueryable<Faction> GetQueryable();
    }
}
