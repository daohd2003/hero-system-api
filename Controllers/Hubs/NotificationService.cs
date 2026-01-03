using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Text.Json;

namespace Controllers.Hubs
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<HeroHub, IChatClient> _hubContext;
        private readonly HttpClient _httpClient;
        private readonly string? _firebaseUrl;

        public NotificationService(IHubContext<HeroHub, IChatClient> hubContext, IConfiguration configuration)
        {
            _hubContext = hubContext;
            _httpClient = new HttpClient();
            _firebaseUrl = configuration["Firebase:RealtimeDbUrl"];
        }

        public async Task SendSystemNotificationAsync(string userId, string message)
        {
            // Lưu vào Firebase theo userId riêng
            if (!string.IsNullOrEmpty(_firebaseUrl))
                await SaveToFirebase(userId, message);

            await _hubContext.Clients.User(userId).ReceiveSystemNotification(message);
        }

        private async Task SaveToFirebase(string userId, string message)
        {
            try
            {
                var notification = new
                {
                    message,
                    timestamp = DateTime.UtcNow.ToString("o")
                };
                var json = JsonSerializer.Serialize(notification);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // Lưu vào path riêng của user: /notifications/{userId}/
                await _httpClient.PostAsync($"{_firebaseUrl}/notifications/{userId}.json", content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase error: {ex.Message}");
            }
        }
    }
}
