using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

public class RcsCallbackRequest
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("areaCode")]
    public string AreaCode { get; set; } = string.Empty;

    [JsonPropertyName("berthCode")]
    public string BerthCode { get; set; } = string.Empty;

    [JsonPropertyName("callCode")]
    public string CallCode { get; set; } = string.Empty;

    [JsonPropertyName("callTyp")]
    public string CallTyp { get; set; } = string.Empty;

    [JsonPropertyName("clientCode")]
    public string ClientCode { get; set; } = string.Empty;

    [JsonPropertyName("cooX")]
    public double CooX { get; set; }

    [JsonPropertyName("cooY")]
    public double CooY { get; set; }

    [JsonPropertyName("ctnrCode")]
    public string CtnrCode { get; set; } = string.Empty;

    [JsonPropertyName("ctnrTyp")]
    public string CtnrTyp { get; set; } = string.Empty;

    [JsonPropertyName("currentCallCode")]
    public string CurrentCallCode { get; set; } = string.Empty;

    [JsonPropertyName("currentPositionCode")]
    public string CurrentPositionCode { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public Dictionary<string, object>? Data { get; set; }

    [JsonPropertyName("dstBinCode")]
    public string DstBinCode { get; set; } = string.Empty;

    [JsonPropertyName("eqpCode")]
    public string EqpCode { get; set; } = string.Empty;

    [JsonPropertyName("indBind")]
    public string IndBind { get; set; } = string.Empty;

    [JsonPropertyName("layer")]
    public string Layer { get; set; } = string.Empty;

    [JsonPropertyName("mapCode")]
    public string MapCode { get; set; } = string.Empty;

    [JsonPropertyName("mapDataCode")]
    public string MapDataCode { get; set; } = string.Empty;

    [JsonPropertyName("mapShortName")]
    public string MapShortName { get; set; } = string.Empty;

    [JsonPropertyName("materialLot")]
    public string MaterialLot { get; set; } = string.Empty;

    [JsonPropertyName("materialType")]
    public string MaterialType { get; set; } = string.Empty;

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("orgCode")]
    public string OrgCode { get; set; } = string.Empty;

    [JsonPropertyName("podCode")]
    public string PodCode { get; set; } = string.Empty;

    [JsonPropertyName("podDir")]
    public string PodDir { get; set; } = string.Empty;

    [JsonPropertyName("podNum")]
    public string PodNum { get; set; } = string.Empty;

    [JsonPropertyName("podTyp")]
    public string PodTyp { get; set; } = string.Empty;

    [JsonPropertyName("relatedArea")]
    public string RelatedArea { get; set; } = string.Empty;

    [JsonPropertyName("reqCode")]
    public string ReqCode { get; set; } = string.Empty;

    [JsonPropertyName("reqTime")]
    public string ReqTime { get; set; } = string.Empty;

    [JsonPropertyName("roadWayCode")]
    public string RoadWayCode { get; set; } = string.Empty;

    [JsonPropertyName("robotCode")]
    public string RobotCode { get; set; } = string.Empty;

    [JsonPropertyName("seq")]
    public string Seq { get; set; } = string.Empty;

    [JsonPropertyName("stgBinCode")]
    public string StgBinCode { get; set; } = string.Empty;

    [JsonPropertyName("subTaskNum")]
    public string SubTaskNum { get; set; } = string.Empty;

    [JsonPropertyName("taskCode")]
    public string TaskCode { get; set; } = string.Empty;

    [JsonPropertyName("taskTyp")]
    public string TaskTyp { get; set; } = string.Empty;

    [JsonPropertyName("tokenCode")]
    public string TokenCode { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("wbCode")]
    public string WbCode { get; set; } = string.Empty;

    [JsonPropertyName("whCode")]
    public string WhCode { get; set; } = string.Empty;
}
