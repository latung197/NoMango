using System.Text.Json.Serialization;
using Wcs.Infrastructure.Data.Models;
using Wcs.Okamura.Enums;

namespace Wcs.Api.Controllers.DTOs;

public class CraneTaskDispatchDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("taskNo")]
    public int TaskNo { get; set; }

    [JsonPropertyName("flowTaskId")]
    public string? FlowTaskId { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = "Automatic";

    [JsonPropertyName("taskType")]
    public string TaskType { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("idempotencyKey")]
    public string IdempotencyKey { get; set; } = string.Empty;

    [JsonPropertyName("completeStatus")]
    public int? CompleteStatus { get; set; }

    [JsonPropertyName("errorCode")]
    public int? ErrorCode { get; set; }

    [JsonPropertyName("errorNote")]
    public string? ErrorNote { get; set; }

    [JsonPropertyName("lastMessage")]
    public string? LastMessage { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("acceptedAt")]
    public DateTime? AcceptedAt { get; set; }

    [JsonPropertyName("completingAt")]
    public DateTime? CompletingAt { get; set; }

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("failedAt")]
    public DateTime? FailedAt { get; set; }

    [JsonPropertyName("qrFinishedRaisedValue")]
    public int? QrFinishedRaisedValue { get; set; }

    [JsonPropertyName("qrFinishedRaisedAt")]
    public DateTime? QrFinishedRaisedAt { get; set; }

    [JsonPropertyName("qrFinishedClearedAt")]
    public DateTime? QrFinishedClearedAt { get; set; }

    [JsonPropertyName("qrFinishedHadCachedQr")]
    public bool? QrFinishedHadCachedQr { get; set; }

    [JsonPropertyName("qrFinishedHadValidQr")]
    public bool? QrFinishedHadValidQr => QrFinishedHadCachedQr;

    [JsonPropertyName("qrFinishedComplete")]
    public bool QrFinishedComplete =>
        QrFinishedRaisedValue == 1 && QrFinishedRaisedAt.HasValue && QrFinishedClearedAt.HasValue;

    public static CraneTaskDispatchDto FromDbModel(CraneTaskDispatchDbModel model)
    {
        var taskType = Enum.IsDefined(typeof(CraneTaskType), model.TaskType)
            ? (CraneTaskType)model.TaskType
            : CraneTaskType.None;

        return new CraneTaskDispatchDto
        {
            Id = model.Id,
            TaskNo = model.TaskNo,
            FlowTaskId = model.FlowTaskId,
            Source = string.IsNullOrWhiteSpace(model.FlowTaskId) ? "Manual" : "Automatic",
            TaskType = taskType.ToString(),
            Status = model.Status.ToString(),
            IdempotencyKey = model.IdempotencyKey,
            CompleteStatus = model.CompleteStatus,
            ErrorCode = model.ErrorCode,
            ErrorNote = model.ErrorNote,
            LastMessage = model.LastMessage,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            AcceptedAt = model.AcceptedAt,
            CompletingAt = model.CompletingAt,
            CompletedAt = model.CompletedAt,
            FailedAt = model.FailedAt,
            QrFinishedRaisedValue = model.QrFinishedRaisedValue,
            QrFinishedRaisedAt = model.QrFinishedRaisedAt,
            QrFinishedClearedAt = model.QrFinishedClearedAt,
            QrFinishedHadCachedQr = model.QrFinishedHadCachedQr
        };
    }
}
