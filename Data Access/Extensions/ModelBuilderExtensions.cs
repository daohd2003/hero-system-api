using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hero>().HasData(
                new Hero
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Iron Man",
                    SuperPower = "Money",
                    Level = 100,
                    PasswordHash = "123456"
                },
                new Hero
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Spider Man",
                    SuperPower = "Agility",
                    Level = 50,
                    PasswordHash = "123456"
                }
            );
        }
    }
}
