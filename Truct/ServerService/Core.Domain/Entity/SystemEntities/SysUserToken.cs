using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Abstractions;

namespace Core.Domain.Entity.SystemEntities
{
    [Table("sys_user_token", Schema = "public")]
    public class SysUserToken: AuditableImpl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(512)]
        [Column("access_token")]
        public string AccessToken { get; set; }

        [MaxLength(512)]
        [Column("refresh_token")]
        public string? RefreshToken { get; set; }

        [Column("issued_at")]
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        [Column("expired_at")]
        public DateTime? ExpiredAt { get; set; }

        [Column("is_revoked")]
        public bool IsRevoked { get; set; } = false;

    }
}
