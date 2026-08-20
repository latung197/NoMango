using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Base response class cho tất cả API responses
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu của data</typeparam>
public class BaseResponse<T>
{
    /// <summary>
    /// Trạng thái thành công của request
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Thông báo mô tả kết quả
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Dữ liệu trả về
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// Timestamp của response
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Mã lỗi (nếu có)
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Chi tiết lỗi (nếu có)
    /// </summary>
    [JsonPropertyName("errors")]
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Tạo response thành công
    /// </summary>
    public static BaseResponse<T> SuccessResult(T data, string message = "Thành công")
    {
        return new BaseResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Tạo response thất bại
    /// </summary>
    public static BaseResponse<T> ErrorResult(string message, string? errorCode = null, List<string>? errors = null)
    {
        return new BaseResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Base response class không có data
/// </summary>
public class BaseResponse : BaseResponse<object>
{
    /// <summary>
    /// Tạo response thành công không có data
    /// </summary>
    public static BaseResponse SuccessResult(string message = "Thành công")
    {
        return new BaseResponse
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Tạo response thất bại không có data
    /// </summary>
    public static new BaseResponse ErrorResult(string message, string? errorCode = null, List<string>? errors = null)
    {
        return new BaseResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}
