
using BusinessObject.Helpers;
using BusinessObject.Models;
using Controllers.Hubs;
using Controllers.Middlewares;
using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;
using Services.Common;
using System.Text;

namespace Controllers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            static IEdmModel GetEdmModel()
            {
                var odataBuilder = new ODataConventionModelBuilder();
                odataBuilder.EntitySet<Hero>("Heroes");
                odataBuilder.EntitySet<Mission>("Missions");
                odataBuilder.EntitySet<Faction>("Factions");

                // Cấu hình composite key cho HeroMission
                var heroMission = odataBuilder.EntitySet<HeroMission>("HeroMissions").EntityType;
                heroMission.HasKey(hm => new { hm.HeroId, hm.MissionId });

                return odataBuilder.GetEdmModel();
            }

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
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IHeroAuthService, HeroAuthService>();

            // Notification Service cho SignalR
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.AddControllers()
            .AddOData(opt => opt
                .Select()   // Cho phép chọn cột ($select)
                .Filter()   // Cho phép lọc ($filter)
                .OrderBy()  // Cho phép sắp xếp ($orderby)
                .Expand()   // Cho phép join bảng ($expand)
                .Count()    // Cho phép đếm ($count)
                .SetMaxTop(100) // Giới hạn lấy tối đa 100 dòng (chống hack)
                .AddRouteComponents("odata", GetEdmModel()) // URL sẽ bắt đầu bằng /odata
            );

            builder.Services.AddSignalR();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hero API", Version = "v1" });

                // Thêm nút Authorize trên Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập token vào đây (không cần gõ 'Bearer ')"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
                    ClockSkew = TimeSpan.Zero // Quan trọng: Loại bỏ độ trễ mặc định 5 phút
                };
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // Nếu request có token và đường dẫn bắt đầu bằng /hubs/hero
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/hero"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Serve static files (chat.html)
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapHub<HeroHub>("/hubs/hero");

            app.Run();
        }
    }
}
