using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Controllers.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Cải tiến 1: Log có cấu trúc (Structured Logging) để dễ filter sau này
                _logger.LogError(ex, "Lỗi hệ thống nghiêm trọng: {Message}", ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Tạo object phản hồi
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error. Please try again later.",
                // Chỉ hiện chi tiết lỗi khi ở môi trường Development
                Detailed = _env.IsDevelopment() ? exception.StackTrace?.ToString() : null,
                Error = _env.IsDevelopment() ? exception.Message : null
            };

            // Cải tiến 2: Sử dụng WriteAsJsonAsync (Hiệu năng cao hơn, tự xử lý Header)
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await context.Response.WriteAsJsonAsync(response, jsonOptions);
        }
    }
}