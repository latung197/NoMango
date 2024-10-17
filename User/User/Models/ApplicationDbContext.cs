using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace User.Models
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
    }
}
