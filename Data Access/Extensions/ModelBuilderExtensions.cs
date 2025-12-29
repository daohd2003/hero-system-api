using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    Level = 100
                },
                new Hero
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Spider Man",
                    SuperPower = "Agility",
                    Level = 50
                }
            );
        }
    }
}
