using Microsoft.Extensions.Logging;

namespace Services.Common
{
    public class ServiceHelper
    {
        private readonly ILogger<ServiceHelper> _logger;

        // Bỏ AppDbContext - chỉ cần Logger, giúp Helper nhẹ và tái sử dụng tốt hơn
        public ServiceHelper(ILogger<ServiceHelper> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Xử lý lỗi 500: chỉ log lỗi, việc rollback để "await using" tự lo
        /// </summary>
        public ServiceResult<T> HandleError<T>(Exception ex, string? context = null)
        {
            _logger.LogError(ex, "[500] Internal Server Error | Context: {Context} | Message: {Message}", 
                context ?? "N/A", ex.Message);
            return ServiceResult<T>.Error(ex.Message);
        }

        /// <summary>
        /// Xử lý lỗi 400
        /// </summary>
        public ServiceResult<T> HandleBadRequest<T>(string message, string? context = null)
        {
            _logger.LogWarning("[400] Bad Request | Context: {Context} | Message: {Message}", 
                context ?? "N/A", message);
            return ServiceResult<T>.BadRequest(message);
        }

        /// <summary>
        /// Xử lý lỗi 404
        /// </summary>
        public ServiceResult<T> HandleNotFound<T>(string message, string? context = null)
        {
            _logger.LogWarning("[404] Not Found | Context: {Context} | Message: {Message}", 
                context ?? "N/A", message);
            return ServiceResult<T>.NotFound(message);
        }
    }
}
