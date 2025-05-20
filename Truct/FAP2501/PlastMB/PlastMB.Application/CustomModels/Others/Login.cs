using System.ComponentModel.DataAnnotations;

namespace PlastMB.Application.CustomModels.Others
{
    public class Login
    {
        [Required]
        [MaxLength(20)]
        public string Username {  get; set; }
        [Required]
        [MaxLength(20)]
        public string Password { get; set; }
    }
}
