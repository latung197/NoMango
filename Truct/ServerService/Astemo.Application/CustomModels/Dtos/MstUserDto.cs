using System.ComponentModel.DataAnnotations;

namespace Astemo.Application.CustomModels.Dtos
{
    public class MstUserDto
    {
        [Required]
        public int user_id { get; set; }
        [Required]
        [MaxLength(100)]
        public string user_name { get; set; }
        [MaxLength(256)]
        public string password { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? email { get; set; }
        [Required]
        [MaxLength(20)]
        public string auth_fl { get; set; } = string.Empty;
        [Required]
        public int enable_fl { get; set; } = 1;
        [Required]
        public string full_name { get; set; }
        [Required]
        public string employee_code { get; set; }
        [MaxLength(20)]
        public DateTime? add_dt { get; set; }
        public DateTime? upd_dt { get; set; }
        public bool Gender { get; set; }
        public int ValidFlg { get; set; } = 1;
        public List<int> Role { get; set; }

    }
}
