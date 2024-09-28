using HRSystem.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HRSystem.WebAPI.Permessions
{
    /// <summary>
    ///  this class for providing custom policy provider &&
    ///  Dynamically creates authorization policies based on permission requirements.
    ///  If a policy name matches a permission pattern, it builds a custom policy that requires the specific permission. 
    ///  It also allows falling back to the default policy provider for non-permission-based policies.
    /// </summary>
    public class PermessionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider {  get; }

        public PermessionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(AppClaim.Permession , StringComparison.CurrentCultureIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermessionRequirement(policyName));
                return await Task.FromResult(policy.Build());
            }

            return await FallbackPolicyProvider.GetPolicyAsync(policyName);
        }


        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return await FallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return await Task.FromResult<AuthorizationPolicy>(null);
        }

    }
}
