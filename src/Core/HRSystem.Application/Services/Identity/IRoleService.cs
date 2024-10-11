using HRSystem.Common.DIContracts;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;

namespace HRSystem.Application.Services.Identity
{
    public interface IRoleService : ITransiantService
    {
        Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest);
    }
}
