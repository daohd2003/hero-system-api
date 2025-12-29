using BusinessObject.DTOs;
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
    public class HeroRepository : IHeroRepository
    {
        private readonly AppDbContext _context;

        public HeroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Hero entity)
        {
            await _context.AddAsync(entity);
        }

        public void Delete(Hero hero)
        {
            _context.Remove(hero);
        }

        public async Task<(List<Hero> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Heroes.CountAsync();
            var items = await _context.Heroes
                .AsNoTracking()
                .Include(h => h.Faction)
                .OrderBy(h => h.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task<Hero?> GetByIdAsync(Guid id)
        {
            return await _context.Heroes.FindAsync(id);
        }

        public void Update(Hero hero)
        {
            _context.Heroes.Update(hero);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<Hero?> GetHeroFullInfoAsync(Guid id)
        {
            var hero = await _context.Heroes
                .Include(h => h.FactionId)
                .Include(h => h.HeroMissions)
                    .ThenInclude(hm => hm.Mission)
                .FirstOrDefaultAsync(h => h.Id == id);
            return hero;
        }
    }
}
