using Core.Domain.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entity
{
    [Table(name: "mst_user", Schema = "public")]
    public class MstUser : AuditableImpl
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        [Required]
        [MaxLength(100)]
        public string user_name { get; set; }
        [Required]
        [MaxLength(256)]
        public string password { get; set; }
        [MaxLength(100)]
        public string? email { get; set; }
        [Required]
        [MaxLength(20)]
        public string auth_fl { get; set; } = string.Empty;
        [Required]
        public int enable_fl { get; set; } = 1;
        [Required]
        [MaxLength(256)]
        public string full_name { get; set; }
        [Required]
        [MaxLength(20)]
        public string employee_code { get; set; }
        public DateTime? add_dt { get; set; }
        public DateTime? upd_dt { get; set; }
        public bool Gender { get; set; }
        public int ValidFlg { get; set; } = 1;
    }
}
