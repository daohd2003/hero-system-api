using Microsoft.AspNetCore.Http;
using Services.Common;

namespace Services
{
    public interface IChatService
    {
        Task<ServiceResult<string>> UploadImageAsync(IFormFile file);
    }
}
