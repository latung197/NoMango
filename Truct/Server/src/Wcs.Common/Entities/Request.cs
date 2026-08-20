using System.Text.Json.Serialization;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class Request
{
    public Guid Id { get; set; }

    public string? FlowTaskId { get; set; }

    public FlowTask? FlowTask { get; set; }

    /// <summary>
    /// Mã request
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Tên khu vực request
    /// </summary>
    public Stage? Stage { get; set; }
    public string StageCode { get; set; } = string.Empty;

    /// <summary>
    /// Tên trạm request
    /// </summary>
    public Station[]? Station { get; set; } = [];

    /// <summary>
    /// Mã vận chuyển
    /// </summary>
    public string? DeliveryCode { get; set; }

    /// <summary>
    /// Thứ tự vận chuyển
    /// </summary>
    public int? DeliveryOrder { get; set; }

    /// <summary>
    /// Trạng thái
    /// </summary>
    public RequestStep CurrentStep { get; set; } = RequestStep.Initial;

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái hoạt động (1 = hiển thị, 0 = không hiển thị/đã xóa)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Danh sách vật liệu request
    /// </summary>
    public List<MaterialRequest> Materials { get; set; } = [];

    /// <summary>
    /// Thời điểm tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm cập nhật lần cuối
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Default constructor
    public Request() { }

    // Main constructor
    public Request(
        string code,
        string stage,
        string? deliveryCode,
        int? deliveryOrder,
        RequestStep currentStep,
        string note,
        bool isActive = true)
    {
        Id = Guid.NewGuid();
        Code = code;
        //Station = station;
        //Stage = stage;
        StageCode = stage;
        DeliveryCode = deliveryCode;
        DeliveryOrder = deliveryOrder;
        CurrentStep = currentStep;
        Note = note;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
