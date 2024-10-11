using HRSystem.Common.Responses.Identity;
using System.Reflection.PortableExecutable;

namespace HRSystem.Common.Requests.Identity
{
    public class UpdateUserRoleRequest
    {
        public string UserId { get; set; }
        public List<UserRolesModelView> Roles { get; set; }
    }
}
