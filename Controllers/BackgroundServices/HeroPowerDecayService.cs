using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Controllers.BackgroundServices
{
    /// <summary>
    /// Background Service: Giảm Level của Hero nếu không hoạt động trong 7 ngày.
    /// 
    /// Concepts học được:
    /// 1. BackgroundService - Base class cho long-running background tasks
    /// 2. IServiceScopeFactory - Tạo scope mới để lấy Scoped services (DbContext)
    /// 3. Scheduled execution - Chạy vào thời điểm cố định (8:00 AM mỗi ngày)
    /// 4. Stoping Token - Graceful shutdown khi app tắt
    /// </summary>
    public class HeroPowerDecayService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HeroPowerDecayService> _logger;

        // Config - có thể đưa ra appsettings
        private readonly TimeSpan _scheduledTime = new TimeSpan(8, 0, 0);   // 8:00 AM mỗi ngày
        private readonly int _inactiveDays = 7;                              // Inactive threshold
        private readonly int _decayPercent = 5;                              // Giảm 5% level

        public HeroPowerDecayService(
            IServiceScopeFactory scopeFactory,
            ILogger<HeroPowerDecayService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Tính thời gian chờ đến lần chạy tiếp theo (8:00 AM)
        /// </summary>
        private TimeSpan GetDelayUntilNextRun()
        {
            var now = DateTime.Now;
            var nextRun = now.Date.Add(_scheduledTime);  // Hôm nay lúc 8:00 AM

            // Nếu đã qua 8:00 AM hôm nay → chờ đến 8:00 AM ngày mai
            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;
            _logger.LogInformation("Next decay check scheduled at {NextRun:yyyy-MM-dd HH:mm:ss} (in {Delay})",
                nextRun, delay);

            return delay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HeroPowerDecayService started. Scheduled daily at {Time}", _scheduledTime);

            while (!stoppingToken.IsCancellationRequested)
            {
                // Tính delay đến 8:00 AM
                var delay = GetDelayUntilNextRun();

                // Chờ đến thời điểm scheduled
                await Task.Delay(delay, stoppingToken);

                // Chạy task
                await ProcessDecayAsync(stoppingToken);
            }
        }

        private async Task ProcessDecayAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Running hero power decay check...");

            try
            {
                // QUAN TRỌNG: BackgroundService là Singleton, nhưng DbContext là Scoped
                // → Phải tạo scope mới để lấy DbContext, không thể inject trực tiếp
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var cutoffDate = DateTime.UtcNow.AddDays(-_inactiveDays);

                // Tìm heroes inactive (LastActiveAt null hoặc quá cũ) và còn level > 1
                var inactiveHeroes = await dbContext.Heroes
                    .Where(h => h.Level > 1 &&
                               (h.LastActiveAt == null || h.LastActiveAt < cutoffDate))
                    .ToListAsync(stoppingToken);

                if (inactiveHeroes.Count == 0)
                {
                    _logger.LogInformation("No inactive heroes found");
                    return;
                }

                foreach (var hero in inactiveHeroes)
                {
                    var oldLevel = hero.Level;
                    // Giảm 5%, tối thiểu còn 1
                    var decay = Math.Max(1, hero.Level * _decayPercent / 100);
                    hero.Level = Math.Max(1, hero.Level - decay);

                    _logger.LogWarning(
                        "Hero '{Name}' inactive since {LastActive:yyyy-MM-dd}. Level: {Old} -> {New}",
                        hero.Name,
                        hero.LastActiveAt?.ToString("yyyy-MM-dd") ?? "never",
                        oldLevel,
                        hero.Level);
                }

                await dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Updated {Count} inactive heroes", inactiveHeroes.Count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Log error nhưng không crash service
                _logger.LogError(ex, "Error during hero power decay");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HeroPowerDecayService stopping...");
            await base.StopAsync(cancellationToken);
        }
    }
}
