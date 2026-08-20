using System.Text.Json.Serialization;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class RequestResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("flowTaskId")]
    public string? FlowTaskId { get; set; }

    [JsonPropertyName("flowTask")]
    public FlowTask? FlowTask { get; set; }

    /// <summary>
    /// Mã request
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    /// <summary>
    /// Tên khu vực request
    /// </summary>
    [JsonPropertyName("stage")]
    public Stage? Stage { get; set; }

    [JsonPropertyName("stageCode")]
    public required string StageCode { get; set; }

    [JsonPropertyName("stageName")]
    public string? StageName { get; set; }

    /// <summary>
    /// Tên trạm request
    /// </summary>
    [JsonPropertyName("station")]
    public Station[]? Station { get; set; } = [];

    /// <summary>
    /// Mã vận chuyển
    /// </summary>
    [JsonPropertyName("deliveryCode")]
    public string? DeliveryCode { get; set; }

    /// <summary>
    /// Thứ tự vận chuyển
    /// </summary>
    [JsonPropertyName("deliveryOrder")]
    public int? DeliveryOrder { get; set; }

    /// <summary>
    /// Trạng thái
    /// </summary>
    [JsonPropertyName("currentStep")]
    public RequestStep CurrentStep { get; set; } = RequestStep.Initial;

    /// <summary>
    /// Ghi chú
    /// </summary>
    [JsonPropertyName("note")]
    public required string Note { get; set; }

    /// <summary>
    /// Danh sách vật liệu yêu cầu
    /// </summary>
    [JsonPropertyName("materials")]
    public List<MaterialRequest> Materials { get; set; } = [];

    /// <summary>
    /// Trạng thái hoạt động (true = hiển thị, false = không hiển thị/đã xóa)
    /// </summary>
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Thời điểm tạo
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Thời điểm cập nhật lần cuối
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public static RequestResponse FromRequest(Request request)
    {
        return new RequestResponse
        {
            Id = request.Id,
            FlowTaskId = request.FlowTaskId,
            FlowTask = request.FlowTask,
            Code = request.Code,
            Stage = request.Stage,
            Station = request.Station,
            StageCode = request.Stage != null ? request.Stage.Code : request.StageCode,
            StageName = request.Stage != null ? request.Stage.Name : string.Empty,
            DeliveryCode = request.DeliveryCode,
            DeliveryOrder = request.DeliveryOrder,
            CurrentStep = request.CurrentStep,
            Note = request.Note,
            Materials = request.Materials,
            IsActive = request.IsActive,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt
        };
    }
}
