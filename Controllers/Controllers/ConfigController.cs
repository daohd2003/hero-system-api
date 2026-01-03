using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Controllers.Controllers
{
    [Route("api")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string? _firebaseUrl;

        public ConfigController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _firebaseUrl = configuration["Firebase:RealtimeDbUrl"];
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications([FromQuery] string userId, [FromQuery] int limit = 20)
        {
            if (string.IsNullOrEmpty(_firebaseUrl) || string.IsNullOrEmpty(userId))
                return Ok(new List<object>());

            try
            {
                var response = await _httpClient.GetStringAsync($"{_firebaseUrl}/notifications/{userId}.json");
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(response);

                if (data == null)
                    return Ok(new List<object>());

                var notifications = data.Values
                    .Select(v => new
                    {
                        message = v.TryGetProperty("message", out var msg) ? msg.GetString() : null,
                        timestamp = v.TryGetProperty("timestamp", out var ts) ? ts.GetString() : null
                    })
                    .Where(n => n.message != null)
                    .OrderBy(n => n.timestamp)
                    .TakeLast(limit)
                    .ToList();

                return Ok(notifications);
            }
            catch
            {
                return Ok(new List<object>());
            }
        }

        [HttpGet("feed-messages")]
        public async Task<IActionResult> GetFeedMessages([FromQuery] int limit = 50)
        {
            if (string.IsNullOrEmpty(_firebaseUrl))
                return Ok(new List<object>());

            try
            {
                var response = await _httpClient.GetStringAsync($"{_firebaseUrl}/feed_messages.json");
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(response);

                if (data == null)
                    return Ok(new List<object>());

                var messages = data.Values
                    .Select(v => new
                    {
                        senderName = v.TryGetProperty("senderName", out var sn) ? sn.GetString() : null,
                        message = v.TryGetProperty("message", out var msg) ? msg.GetString() : null,
                        timestamp = v.TryGetProperty("timestamp", out var ts) ? ts.GetString() : null
                    })
                    .Where(m => m.message != null && m.senderName != null)
                    .OrderBy(m => m.timestamp)
                    .TakeLast(limit)
                    .ToList();

                return Ok(messages);
            }
            catch
            {
                return Ok(new List<object>());
            }
        }
    }
}
