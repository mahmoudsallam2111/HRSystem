using HRSystem.Application.Services.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Roles.Commands;

public class DeleteRoleCommand : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}


public class DeleteRoleHandler(IRoleService _roleService)
    : IRequestHandler<DeleteRoleCommand, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        return _roleService.DeleteRoleAsync(request.RoleId);
    }
}