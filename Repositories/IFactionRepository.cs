using BusinessObject.Models;

namespace Repositories
{
    public interface IFactionRepository
    {
        Task AddAsync(Faction faction);
        void UpdateAsync(Faction faction);
        void DeleteAsync(Faction faction);
        Task<List<Faction>> GetAllAsync();
        Task<Faction?> GetByIdAsync(Guid id);
    }
}
