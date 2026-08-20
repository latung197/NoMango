using System.ComponentModel.DataAnnotations;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Request DTO cho vị trí Crane
/// </summary>
public class CranePositionRequest
{
    /// <summary>
    /// Cột (Row)
    /// </summary>
    [Required(ErrorMessage = "Row là bắt buộc")]
    public int Row { get; set; }

    /// <summary>
    /// Tầng (Floor)
    /// </summary>
    [Required(ErrorMessage = "Floor là bắt buộc")]
    public int Floor { get; set; }

    /// <summary>
    /// Vị trí Position
    /// </summary>
    [Required(ErrorMessage = "Position là bắt buộc")]
    public int Position { get; set; }

    /// <summary>
    /// Lấy mã vị trí Crane
    /// </summary>
    public string GetPositionCode() => $"F{Floor}-R{Row}-P{Position}";
}

