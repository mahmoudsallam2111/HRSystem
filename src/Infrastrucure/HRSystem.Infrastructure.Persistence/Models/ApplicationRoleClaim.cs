using Microsoft.AspNetCore.Identity;

namespace HRSystem.Infrastructure.Persistence.Models
{
    /// <summary>
    /// extend IdentityRoleClaim class
    /// </summary>
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public string Description { get; set; }
        public string Group { get; set; }   // idntify a group permessions
    }
}
