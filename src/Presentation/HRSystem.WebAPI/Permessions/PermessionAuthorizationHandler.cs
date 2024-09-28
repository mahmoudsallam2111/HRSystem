using HRSystem.Common.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace HRSystem.WebAPI.Permessions
{
    /// <summary>
    /// validate that the new added permesion meets the specifications
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
