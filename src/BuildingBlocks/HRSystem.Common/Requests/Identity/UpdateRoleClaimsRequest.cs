using HRSystem.Common.Responses.Identity;

namespace HRSystem.Common.Requests.Identity
{
    public class UpdateRoleClaimsRequest
    {
        public string RoleId { get; set; }
        public List<RoleClaimViewModel> RoleClaims { get; set; }
    }
}
