namespace Services.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data) => new() { Success = true, StatusCode = 200, Data = data };
        public static ServiceResult<T> Created(T data) => new() { Success = true, StatusCode = 201, Data = data };
        public static ServiceResult<T> BadRequest(string message) => new() { Success = false, StatusCode = 400, Message = message };
        public static ServiceResult<T> NotFound(string message) => new() { Success = false, StatusCode = 404, Message = message };
        public static ServiceResult<T> Error(string message) => new() { Success = false, StatusCode = 500, Message = message };
    }
}
