using Microsoft.AspNetCore.Authorization;

namespace HRSystem.WebAPI.Permessions
{
    /// <summary>
    ///  this the requirement class needed for PermessionAuthenticationHandler class
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
