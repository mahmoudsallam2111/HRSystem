using HRSystem.Common.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace HRSystem.WebAPI.Permessions
{
    /// <summary>
    /// Contains the logic for checking if a user meets the PermessionRequirement.
    /// It examines the user's claims and checks if they have the necessary permission (specified in PermessionRequirement)
    /// </summary>
    public class PermessionAuthorizationHandler : AuthorizationHandler<PermessionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermessionRequirement requirement)
        {
            if (context.User is null)
            {
                await Task.CompletedTask;
            }

            var permessions = context.User.Claims
                .Where(c => c.Type == AppClaim.Permession &&
                c.Value == requirement.Permession
                && c.Issuer == "LOCAL AUTHORITY");

            if (permessions.Any())
            {
                context.Succeed(requirement);
                await Task.CompletedTask;
            }
        }
    }
}
