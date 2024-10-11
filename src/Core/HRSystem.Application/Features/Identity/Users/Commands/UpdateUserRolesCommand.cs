using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Users.Commands;

public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
{
    public UpdateUserRoleRequest UpdateUserRoleRequest { get; set; }
}




public class UpdateUserRolesHandler : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserRolesHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserRolesAsync(request.UpdateUserRoleRequest);
    }
}