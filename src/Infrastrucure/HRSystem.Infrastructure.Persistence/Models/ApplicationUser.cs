using Microsoft.AspNetCore.Identity;

namespace HRSystem.Infrastructure.Persistence.Models
{
    /// <summary>
    /// extend idenity user by adding additional properties
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
