using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Application.Common.Dtos
{
    // Dạng có generic, dùng cho mọi response có trả về data kiểu T
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new ApiResponse<T> { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message, object? errors = null) =>
            new ApiResponse<T> { Success = false, Message = message, Errors = errors };
    }

    // Dạng không generic, dùng cho trường hợp không cần trả dữ liệu (chỉ message hoặc lỗi)
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Errors { get; set; }

        public static ApiResponse Ok(string? message = null) =>
            new ApiResponse { Success = true, Message = message };

        public static ApiResponse Fail(string message, object? errors = null) =>
            new ApiResponse { Success = false, Message = message, Errors = errors };
    }
}
