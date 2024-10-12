using HRSystem.Common.DIContracts;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;

namespace HRSystem.Application.Services.Identity
{
    public interface IRoleService : ITransiantService
    {
        Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest);
        Task<IResponseWrapper> GetAllRolesAsync();
        Task<IResponseWrapper> GetRoleByIdAsync(string RoleId);
        Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRoleRequest);
        Task<IResponseWrapper> DeleteRoleAsync(string RoleId);
        Task<IResponseWrapper> GetPermessionsAsync(string RoleId);
        Task<IResponseWrapper> UpdateRolePermessionsAsync(UpdateRoleClaimsRequest updateRoleClaimsRequest);
    }
}
