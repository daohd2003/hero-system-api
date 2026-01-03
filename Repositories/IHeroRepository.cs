using BusinessObject.Models;

namespace Repositories
{
    public interface IHeroRepository
    {
        Task<(List<Hero> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<Hero?> GetByIdAsync(Guid id);
        Task AddAsync(Hero entity);
        void Update(Hero hero);
        void Delete(Hero hero);
        Task<Hero?> GetHeroFullInfoAsync(Guid id);

        Task<Hero?> GetHeroByName(string name);
        IQueryable<Hero> GetQueryable();
    }
}
