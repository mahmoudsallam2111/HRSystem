using HRSystem.Common.DIContracts;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;

namespace HRSystem.Application.Services.Identity
{
    public interface IUserService : ITransiantService
    {
        Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest);
        Task<IResponseWrapper> GetUserByIdAsync(string id);
        Task<IResponseWrapper> GetAllUsersAsync();
        Task<IResponseWrapper> UpdateUserAsync( UpdateUserRequest updateUserRequest);
        Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest changePasswordRequest);
        Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest changeUserStatusRequest);
        Task<IResponseWrapper> GetRolesAsyn(string userId);
        Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRoleRequest updateUserRoleRequest);
    }
}
