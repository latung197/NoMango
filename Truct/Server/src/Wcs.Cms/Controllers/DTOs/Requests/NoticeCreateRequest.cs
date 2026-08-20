using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class NoticeCreateRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}

public class OpcTagCreateRequest
{
    // node_id
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("nodeId")]
    public required string NodeId { get; set; }

    // old_value
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("oldValue")]
    public required string OldValue { get; set; }

    // new_value
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("newValue")]
    public required string NewValue { get; set; }
}

public class FlowTaskIdRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    //[JsonPropertyName("flow_task_id")]
    public required string FlowTaskId { get; set; }
}