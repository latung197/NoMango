using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Responses;

/// <summary>
/// Meta information cho pagination
/// </summary>
public class Meta
{
    /// <summary>
    /// Tổng số trang
    /// </summary>
    [JsonPropertyName("totalPage")]
    public int TotalPage { get; set; }

    /// <summary>
    /// Trang hiện tại
    /// </summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    /// <summary>
    /// Kích thước trang
    /// </summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }

    /// <summary>
    /// Tổng số bản ghi
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
}

/// <summary>
/// Base response class cho tất cả API responses
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu của data</typeparam>
public class BaseResponse<T>
{
    /// <summary>
    /// Trạng thái của request (success, error, etc.)
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

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
    /// Meta information cho pagination (optional)
    /// </summary>
    [JsonPropertyName("meta")]
    public Meta? Meta { get; set; }

    /// <summary>
    /// Tạo response thành công
    /// </summary>
    public static BaseResponse<T> SuccessResult(T data, string message = "Thành công", Meta? meta = null)
    {
        return new BaseResponse<T>
        {
            Status = "success",
            Message = message,
            Data = data,
            Meta = meta
        };
    }

    /// <summary>
    /// Tạo response thất bại
    /// </summary>
    public static BaseResponse<T> ErrorResult(string message, string status = "error")
    {
        return new BaseResponse<T>
        {
            Status = status,
            Message = message
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
            Status = "success",
            Message = message
        };
    }

    /// <summary>
    /// Tạo response thất bại không có data
    /// </summary>
    public static new BaseResponse ErrorResult(string message, string status = "error")
    {
        return new BaseResponse
        {
            Status = status,
            Message = message
        };
    }
}
