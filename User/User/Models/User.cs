using Microsoft.AspNetCore.Identity;

namespace User.Models
{
    public class User:IdentityUser
    {
        public string? Inttials {  get; set; }
    }
}
