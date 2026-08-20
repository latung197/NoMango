using System.Text.Json.Serialization;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class ErrorResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("code")]
    public required string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("type")]
    public int ErrorType { get; set; }

    //[JsonPropertyName("stack_trace")]
    //public string? StackTrace { get; set; }

    //[JsonPropertyName("action_required")]
    //public string? ActionRequired { get; set; }

    //[JsonPropertyName("error_date")]
    //public DateTime ErrorDate { get; set; }

    //[JsonPropertyName("resolved_date")]
    //public DateTime? ResolvedDate { get; set; }

    //[JsonPropertyName("is_resolved")]
    //public bool IsResolved { get; set; }

    [JsonPropertyName("area")]
    public string? Area { get; set; } = string.Empty;

    [JsonPropertyName("flow_task_id")]
    public string? FlowTaskId { get; set; }

    [JsonPropertyName("from_station")]
    public string? FromStation { get; set; }

    [JsonPropertyName("to_station")]
    public string? ToStation { get; set; }

    //[JsonPropertyName("stage")]
    //public string? Stage { get; set; } = string.Empty;

    //[JsonPropertyName("station")]
    //public string? Station { get; set; } = string.Empty;

    //[JsonPropertyName("module")]
    //public string? ModuleName { get; set; } = string.Empty;

    //[JsonPropertyName("method")]
    //public string? MethodName { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public int Status { get; set; } = 0;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public static ErrorResponse FromError(Error error)
    {
        return new ErrorResponse
        {
            Id = error.Id,
            Code = error.Code,
            Message = error.Message,
            ErrorType = (int)error.ErrorType,
            //StackTrace = error.StackTrace,
            //ActionRequired = error.ActionRequired,
            //ErrorDate = error.ErrorDate,
            //ResolvedDate = error.ResolvedDate,
            //IsResolved = error.IsResolved,
            Area = error.Area?.ToString(),
            FlowTaskId = error.FlowTaskId,
            FromStation = error.FromStation,
            ToStation = error.ToStation,
            //Stage = error.Stage,
            //Station = error.Station,
            Status = (int)error.Status,
            CreatedAt = error.CreatedAt,
            UpdatedAt = error.UpdatedAt,
        };
    }
}