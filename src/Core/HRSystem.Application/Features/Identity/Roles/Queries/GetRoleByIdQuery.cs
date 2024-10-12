using HRSystem.Application.Services.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Roles.Queries;

public class GetRoleByIdQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetRoleByIdHandler(IRoleService _roleService)
    : IRequestHandler<GetRoleByIdQuery, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        return _roleService.GetRoleByIdAsync(request.RoleId);
    }
}
