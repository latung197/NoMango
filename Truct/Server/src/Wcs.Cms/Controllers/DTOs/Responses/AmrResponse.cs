using System.Text.Json.Serialization;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class AmrResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("area")]
    public string Area { get; set; } = string.Empty;

    [JsonPropertyName("mapCode")]
    public string MapCode { get; set; } = string.Empty;

    [JsonPropertyName("mapName")]
    public string MapName { get; set; } = string.Empty;

    [JsonPropertyName("robotStatus")]
    public string RobotStatus { get; set; } = string.Empty;

    [JsonPropertyName("typeCode")]
    public string TypeCode { get; set; } = string.Empty;

    [JsonPropertyName("battery")]
    public string Battery { get; set; } = string.Empty;

    [JsonPropertyName("direction")]
    public string Direction { get; set; } = string.Empty;

    [JsonPropertyName("exclude")]
    public string Exclude { get; set; } = string.Empty;

    [JsonPropertyName("excludeStr")]
    public string ExcludeStr { get; set; } = string.Empty;

    [JsonPropertyName("onLine")]
    public string OnLine { get; set; } = string.Empty;

    [JsonPropertyName("podCode")]
    public string PodCode { get; set; } = string.Empty;

    [JsonPropertyName("podDir")]
    public string PodDir { get; set; } = string.Empty;

    [JsonPropertyName("posX")]
    public string PosX { get; set; } = string.Empty;

    [JsonPropertyName("posY")]
    public string PosY { get; set; } = string.Empty;

    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("statusStr")]
    public string StatusStr { get; set; } = string.Empty;

    [JsonPropertyName("stop")]
    public string Stop { get; set; } = string.Empty;

    [JsonPropertyName("stopStr")]
    public string StopStr { get; set; } = string.Empty;

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public static AmrResponse FromAmr(Amr amr)
    {
        return new AmrResponse
        {
            Id = amr.Id,
            Name = amr.Name,
            Code = amr.Code,
            Area = amr.Area != null ? amr.Area.ToString() : string.Empty,
            MapCode = amr.MapCode,
            MapName = amr.MapName,
            RobotStatus = amr.RobotStatus,
            TypeCode = amr.TypeCode,
            Battery = amr.Battery,
            Direction = amr.Direction,
            Exclude = amr.Exclude,
            ExcludeStr = amr.ExcludeStr,
            OnLine = amr.OnLine,
            PodCode = amr.PodCode,
            PodDir = amr.PodDir,
            PosX = amr.PosX,
            PosY = amr.PosY,
            Ip = amr.Ip,
            Status = amr.Status,
            StatusStr = amr.StatusStr,
            Stop = amr.Stop,
            StopStr = amr.StopStr,
            IsActive = amr.IsActive,
            CreatedAt = amr.CreatedAt,
            UpdatedAt = amr.UpdatedAt,
        };
    }
}