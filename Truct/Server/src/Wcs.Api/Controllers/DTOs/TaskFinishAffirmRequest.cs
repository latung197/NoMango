using System.ComponentModel.DataAnnotations;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Request ghi giá trị D3013 (wcs_taskFinishAffirm).
/// 0=None, 1=AutoCompleted, 2=ManualCompleted, 3=Canceled
/// </summary>
public class TaskFinishAffirmRequest
{
    [Range(0, 3, ErrorMessage = "Giá trị D3013 phải từ 0 đến 3")]
    public int Value { get; set; }
}
