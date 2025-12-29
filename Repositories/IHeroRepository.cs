using BusinessObject.DTOs;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IHeroRepository
    {
        Task<(List<Hero> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<Hero?> GetByIdAsync(Guid id);
        Task AddAsync(Hero entity);
        void Update(Hero hero);
        void Delete(Hero hero);
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<Hero?> GetHeroFullInfoAsync(Guid id);
    }
}
