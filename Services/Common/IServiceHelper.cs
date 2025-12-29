namespace Services.Common
{
    public interface IServiceHelper
    {
        ServiceResult<T> HandleError<T>(Exception ex, string? context = null);
        ServiceResult<T> HandleBadRequest<T>(string message, string? context = null);
        ServiceResult<T> HandleNotFound<T>(string message, string? context = null);
    }
}
