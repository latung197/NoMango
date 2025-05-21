using System.ComponentModel.DataAnnotations;

namespace Astemo.Application.CustomModels.Others
{
    public class ChangePassword
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Password { get; set; }
        [Required]
        [MaxLength(20)]
        public string NewPassword { get; set; }
        [Required]
        [MaxLength(20)]
        public string NewPassword2 { get; set; }
    }
}
