using Microsoft.AspNetCore.Authorization;

namespace HRSystem.WebAPI.Permessions
{
    /// <summary>
    /// Defines a custom requirement that specifies a permission the user must have to access certain resources &&
    /// this the requirement class needed for PermessionAuthenticationHandler class
    /// </summary>
    public class PermessionRequirement : IAuthorizationRequirement
    {
        public string Permession { get; set; }
        public PermessionRequirement(string permession)
        {

            Permession = permession;

        }
    }
}
