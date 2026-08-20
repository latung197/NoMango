using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("Amrs")]
public class AmrDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(16)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(16)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public StageArea Area { get; set; } = StageArea.Chip;

    [MaxLength(2)]
    public string MapCode { get; set; } = string.Empty;

    [MaxLength(32)]
    public string MapName { get; set; } = string.Empty;

    [MaxLength(10)]
    public string RobotStatus { get; set; } = string.Empty;

    [MaxLength(30)]
    public string TypeCode { get; set; } = string.Empty;

    [MaxLength(3)]
    public string Battery { get; set; } = string.Empty;

    [MaxLength(4)]
    public string Direction { get; set; } = string.Empty;

    [MaxLength(2)]
    public string Exclude { get; set; } = string.Empty;

    [MaxLength(10)]
    public string ExcludeStr { get; set; } = string.Empty;

    [MaxLength(2)]
    public string OnLine { get; set; } = string.Empty;

    [MaxLength(2)]
    public string PodCode { get; set; } = string.Empty;

    [MaxLength(10)]
    public string PodDir { get; set; } = string.Empty;

    [MaxLength(10)]
    public string PosX { get; set; } = string.Empty;

    [MaxLength(10)]
    public string PosY { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Ip { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(255)]
    public string StatusStr { get; set; } = string.Empty;

    [MaxLength(2)]
    public string Stop { get; set; } = string.Empty;

    [MaxLength(255)]
    public string StopStr { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
