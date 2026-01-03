using Microsoft.AspNetCore.Http;
using Services.Common;

namespace Services
{
    public class ChatService : IChatService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IServiceHelper _serviceHelper;

        // Config có thể đưa ra appsettings nếu cần
        private static readonly string[] AllowedTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public ChatService(ICloudinaryService cloudinaryService, IServiceHelper serviceHelper)
        {
            _cloudinaryService = cloudinaryService;
            _serviceHelper = serviceHelper;
        }

        public async Task<ServiceResult<string>> UploadImageAsync(IFormFile file)
        {
            // Validate file
            if (file == null || file.Length == 0)
                return _serviceHelper.HandleBadRequest<string>("Chưa chọn file");

            if (!AllowedTypes.Contains(file.ContentType))
                return _serviceHelper.HandleBadRequest<string>("Chỉ hỗ trợ ảnh JPEG, PNG, GIF, WEBP");

            if (file.Length > MaxFileSize)
                return _serviceHelper.HandleBadRequest<string>("File quá lớn (tối đa 5MB)");

            try
            {
                using var stream = file.OpenReadStream();
                var imageUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName);

                if (string.IsNullOrEmpty(imageUrl))
                    return _serviceHelper.HandleError<string>(new Exception("Upload thất bại"));

                return ServiceResult<string>.Ok(imageUrl);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<string>(ex, "Upload image");
            }
        }
    }
}
