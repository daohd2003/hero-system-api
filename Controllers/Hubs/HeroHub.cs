using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Controllers.Hubs
{
    [Authorize]
    public class HeroHub : Hub<IChatClient>
    {
        private readonly HttpClient _httpClient;
        private readonly string _firebaseUrl;

        public HeroHub(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _firebaseUrl = configuration["Firebase:RealtimeDbUrl"] 
                ?? throw new ArgumentNullException("Firebase:RealtimeDbUrl is not configured");
        }

        public async Task SendMessageToFeed(string message)
        {
            var senderName = Context.User?.Identity?.Name ?? "Anonymous";
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var timestamp = DateTime.UtcNow;

            // Lưu vào Firebase
            var chatMessage = new
            {
                senderId,
                senderName,
                message,
                timestamp = timestamp.ToString("o"),
                type = "feed"
            };
            await SaveToFirebase("feed_messages", chatMessage);

            await Clients.All.ReceiveFeedMessage(senderName, message, timestamp);
        }

        public async Task SendPrivateMessage(string targetHeroId, string message)
        {
            var senderName = Context.User?.Identity?.Name ?? "Anonymous";
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var timestamp = DateTime.UtcNow;

            // Tạo conversation key (sắp xếp để 2 người luôn có cùng key)
            var ids = new[] { senderId, targetHeroId }.OrderBy(x => x).ToArray();
            var conversationKey = $"{ids[0]}_{ids[1]}";

            // Lưu vào Firebase
            var chatMessage = new
            {
                senderId,
                senderName,
                targetHeroId,
                message,
                timestamp = timestamp.ToString("o"),
                type = "private"
            };
            await SaveToFirebase($"private_messages/{conversationKey}", chatMessage);

            await Clients.User(targetHeroId).ReceivePrivateMessage(senderName, message, timestamp);
            await Clients.Caller.ReceivePrivateMessage("Me", message, timestamp);
        }

        public override async Task OnConnectedAsync()
        {
            var name = Context.User?.Identity?.Name;
            
            // Lưu notification vào Firebase
            var notification = new
            {
                message = $"{name} đã tham gia Server.",
                timestamp = DateTime.UtcNow.ToString("o")
            };
            await SaveToFirebase("notifications", notification);

            await Clients.All.ReceiveSystemNotification($"{name} đã tham gia Server.");
            await base.OnConnectedAsync();
        }

        private async Task SaveToFirebase(string path, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync($"{_firebaseUrl}/{path}.json", content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase error: {ex.Message}");
            }
        }
    }
}
