using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("Errors")]
public class ErrorDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(15)]
    public required string Code { get; set; }

    [MaxLength(500)]
    public string? Message { get; set; }

    [MaxLength(50)]
    //public string? ErrorType { get; set; }
    public ErrorType ErrorType { get; set; } = ErrorType.Warning;

    //public string? StackTrace { get; set; }

    //[MaxLength(500)]
    //public string? ActionRequired { get; set; }

    //public DateTime ErrorDate { get; set; } = DateTime.UtcNow;

    //public DateTime? ResolvedDate { get; set; }

    //[Required]
    //public bool IsResolved { get; set; } = false;

    [MaxLength(50)]
    public string? Area { get; set; }
    //public StageArea? Area { get; set; }

    [MaxLength(10)]
    public string? FlowTaskId { get; set; }

    [MaxLength(50)]
    public string? FromStation { get; set; }

    [MaxLength(50)]
    public string? ToStation { get; set; }

    //[MaxLength(50)]
    //public string? Stage { get; set; }

    //[MaxLength(50)]
    //public string? Station { get; set; }

    //[MaxLength(100)]
    //public string? ModuleName { get; set; }

    //[MaxLength(200)]
    //public string? MethodName { get; set; }

    //[MaxLength(255)]
    //public string Note { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public ErrorStatus Status { get; set; } = ErrorStatus.New;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
