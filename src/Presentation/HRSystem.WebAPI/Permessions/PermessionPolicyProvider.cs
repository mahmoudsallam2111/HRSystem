using HRSystem.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HRSystem.WebAPI.Permessions
{
    /// <summary>
    /// add permession dynamically in permession const
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
