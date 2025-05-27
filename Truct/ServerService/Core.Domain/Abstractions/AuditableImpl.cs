using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Abstractions
{
    public class AuditableImpl : IAuditable
    {
        [MaxLength(14)]
        public string? CreateTime { get; set; }
        [MaxLength(5)]
        public string? CreateId { get; set; }
        [MaxLength(14)]
        public string? UpdateTime { get; set; }
        [MaxLength(5)]
        public string? UpdateId { get; set; }
    }
}
