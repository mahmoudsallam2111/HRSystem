using Microsoft.AspNetCore.Identity;

namespace HRSystem.Infrastructure.Persistence.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
    }
}
