namespace FinancieraSoluciones.Application.DTOs.Shared
{
    public class ApiResponse<T>
    {
        public int HttpCode { get; set; }
        public bool HasError { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
        public T Result { get; set; } = default!;

        public static ApiResponse<T> Success(T result, string message = "", int httpCode = 200)
        {
            return new ApiResponse<T>
            {
                HttpCode = httpCode,
                HasError = false,
                Message = message ?? string.Empty,
                ErrorCode = 0,
                Result = result
            };
        }

        public static ApiResponse<T> Fail(string message, int httpCode = 400, int errorCode = 0)
        {
            return new ApiResponse<T>
            {
                HttpCode = httpCode,
                HasError = true,
                Message = message ?? string.Empty,
                ErrorCode = errorCode,
                Result = default!
            };
        }
    }
}

