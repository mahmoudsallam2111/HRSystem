using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Identity;

namespace HRSystem.Infrastructure.Persistence.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request)
        {
            var IsRoleExist = await _roleManager.FindByNameAsync(request.Name);
            if (IsRoleExist is not null)
                return await ResponseWrapper.FailAsync("Role already Exist");

            var newRole = new ApplicationRole()
            {
                Name = request.Name,
                Description = request.Description
            };

          var identityResult =   await _roleManager.CreateAsync(newRole);

            if (identityResult.Succeeded)
            {
                return await ResponseWrapper.SuccessAsync("Craete Role Successfully");
            }

            return await ResponseWrapper.FailAsync("failed to create the new role");
        }
    }
}
