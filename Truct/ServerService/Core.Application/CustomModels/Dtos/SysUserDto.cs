using Core.Domain.Entity.SystemEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Application.CustomModels.Dtos
{
    public class SysUserDto
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("user_name")]
        [StringLength(100)] // ud_name thường là 100 ký tự
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("full_name")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Column("ma_dvcs")]
        [StringLength(20)]
        public string MaDvcs { get; set; } = "DVCS01";

        [Column("email")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Column("phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("employee_code")]
        public string EmployeeCode { get; set; }

        [Column("auth_fl")]
        [MaxLength(25)]
        public string AuthFl { get; set; }

        [Column("enablefl")]
        public int EnableFl { get; set; }

        [Column("validflg")]
        public int ValidFlg { get; set; }

        [Column("gender")]
        public bool GenDer { get; set; }

        // Navigation properties
        public virtual ICollection<SysUserRole> UserRoles { get; set; } = new List<SysUserRole>();
        public virtual ICollection<SysUserCommand> UserPermissions { get; set; } = new List<SysUserCommand>();
        public virtual ICollection<SysUserToken> Tokens { get; set; } = new List<SysUserToken>();


    }
}
