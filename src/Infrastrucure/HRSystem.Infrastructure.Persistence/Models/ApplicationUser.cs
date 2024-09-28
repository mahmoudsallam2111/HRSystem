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
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
    }
}
