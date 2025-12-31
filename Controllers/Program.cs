
using BusinessObject.Helpers;
using Controllers.Middlewares;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;
using Services.Common;

namespace Controllers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Add services to the container.
            builder.Services.AddScoped<IServiceHelper, ServiceHelper>();

            // Unit of Work (quản lý SaveChanges và Transaction)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            builder.Services.AddScoped<IHeroService, HeroService>();
            builder.Services.AddScoped<IMissionService, MissionService>();
            builder.Services.AddScoped<IFactionService, FactionService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
