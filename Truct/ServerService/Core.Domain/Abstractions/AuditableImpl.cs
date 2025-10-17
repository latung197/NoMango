using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Abstractions
{
    public class AuditableImpl : IAuditable
    {
        [MaxLength(14)]
        [Column("createtime")]
        public string? CreateTime { get; set; }
        [MaxLength(5)]
        [Column("createid")]
        public string? CreateId { get; set; }
        [MaxLength(14)]
        [Column("updatetime")]
        public string? UpdateTime { get; set; }
        [MaxLength(5)]
        [Column("updateid")]
        public string? UpdateId { get; set; }
    }
}
