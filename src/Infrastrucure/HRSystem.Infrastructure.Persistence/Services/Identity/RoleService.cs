using AutoMapper;
using HRSystem.Application.Services.Identity;
using HRSystem.Common.Authorization;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Identity;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Infrastructure.Persistence.Context;
using HRSystem.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HRSystem.Infrastructure.Persistence.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public RoleService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _dbContext = dbContext;
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

        public async Task<IResponseWrapper> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            if (roles is not null)
            {
                var mappedRoles = _mapper.Map<List<RoleResponse>>(roles);
                return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(mappedRoles);
            }

            return await ResponseWrapper.FailAsync("failed to get roles");
        }

        public async Task<IResponseWrapper> GetRoleByIdAsync(string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role is not null)
            {
                var mappedRole = _mapper.Map<RoleResponse>(role);
                return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedRole);
            }

            return await ResponseWrapper.FailAsync("failed to get role");
        }

        public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRoleRequest)
        {
           var roleToUpdate = await _roleManager.FindByIdAsync(updateRoleRequest.RoleId);

           if (roleToUpdate is null)
                return await ResponseWrapper.FailAsync("failed to get roles");

            if (roleToUpdate.Name == AppRoles.Admin)
                return await ResponseWrapper.FailAsync("Update Admin Role Is not Allowed");
             
              roleToUpdate.Name = updateRoleRequest.Name;
              roleToUpdate.Description = updateRoleRequest.Description;

             var result = await _roleManager.UpdateAsync(roleToUpdate);

            if (result.Succeeded)
            {
                var mappedRole = _mapper.Map<RoleResponse>(roleToUpdate);
                return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedRole);
            }

            return await ResponseWrapper.FailAsync("failed to update role");
        }

        public async Task<IResponseWrapper> DeleteRoleAsync(string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role is null)
            {
               return await ResponseWrapper.FailAsync("failed to get role");
            }

          var result =  await _roleManager.DeleteAsync(role);   
           if (result.Succeeded)
                return await ResponseWrapper.SuccessAsync($"{role.Name} Role deleted Successfully");

            return await ResponseWrapper.FailAsync("failed to delete role");
        }

        public async Task<IResponseWrapper> GetPermessionsAsync(string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role is null)
                return await ResponseWrapper.FailAsync("failed to get role");


            var allPermessions = AppPermessions.AllPermessions;

            var roleClaimResponse = new RoleClaimResponse
            {
                Role = new()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                },

                RoleClaims = new()

            };

            var currentRoleClaims  = await GetAllClaimsForRole(RoleId);

            var allPermessionsNames = allPermessions.Select(x => x.Name).ToList();
            var currentRoleClaimsValues = currentRoleClaims.Select(x => x.ClaimValue).ToList();

            var currentlyAssignedRoleClaimsNames = allPermessionsNames
                                                            .Intersect(currentRoleClaimsValues)
                                                            .ToList();

            foreach (var permession in allPermessions)
            {
                if (currentlyAssignedRoleClaimsNames.Any(c => c == permession.Name))
                {
                    roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                    {
                        RoleId = RoleId,
                        ClaimType = AppClaim.Permession,
                        ClaimValue = permession.Name,
                        Description = permession.Description,
                        Group = permession.Group,
                        IsAssignedToRole = true
                    });
                }
                else
                {
                    roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                    {
                        RoleId = RoleId,
                        ClaimType = AppClaim.Permession,
                        ClaimValue = permession.Name,
                        Description = permession.Description,
                        Group = permession.Group,
                        IsAssignedToRole = false
                    });
                }
                
            }

            return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }


        private async Task<List<RoleClaimViewModel>> GetAllClaimsForRole(string roleId)
        {
            var roleClaims = await _dbContext.RoleClaims
                .Where(rc=>rc.RoleId == roleId)
                .ToListAsync();

            if (roleClaims.Any())
            {
                 var mappedRoleClaims = _mapper.Map<List<RoleClaimViewModel>>(roleClaims);  
                return mappedRoleClaims;
            }

            return new List<RoleClaimViewModel>();
        }

        public async Task<IResponseWrapper> UpdateRolePermessionsAsync(UpdateRoleClaimsRequest updateRoleClaimsRequest)
        {
            var role = await _roleManager.FindByIdAsync(updateRoleClaimsRequest.RoleId);
            if (role is null)
                return await ResponseWrapper.FailAsync("failed to get role");

            if (role.Name == AppRoles.Admin)
                return await ResponseWrapper<string>.FailAsync("Update Admin Permessions Is not Allowed");
            
            var assignedPermessions = updateRoleClaimsRequest
                                                  .RoleClaims
                                                  .Where(rc=>rc.IsAssignedToRole)
                                                  .ToList();

            var currentlyAssinedClaims = await _roleManager.GetClaimsAsync(role);

            try
            {
                foreach (var cliam in currentlyAssinedClaims)
                {
                    await _roleManager.RemoveClaimAsync(role, cliam);
                }

                foreach (var cliam in assignedPermessions)
                {
                    var mappedRoleClaims = _mapper.Map<ApplicationRoleClaim>(cliam);
                    await _dbContext.RoleClaims.AddAsync(mappedRoleClaims);
                    await _dbContext.SaveChangesAsync();
                }

                return await ResponseWrapper<string>.SuccessAsync("Update permessions performed successfully");
            }
            catch (Exception ex)
            {

                return await ResponseWrapper<string>.FailAsync("Update permessions failed");
            }

           
        }
    }
}
