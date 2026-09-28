using System.ComponentModel.DataAnnotations;

namespace Core.Application.CustomModels.Others
{
    public class Login
    {
        [Required]
        [MaxLength(100)]
        public string Username {  get; set; }
        [Required]
        [MaxLength(128)]
        public string Password { get; set; }
    }
}
