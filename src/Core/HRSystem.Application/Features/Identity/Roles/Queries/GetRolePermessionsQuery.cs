using HRSystem.Application.Services.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Roles.Queries;

public class GetRolePermessionsQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetRolePermessionsHandler : IRequestHandler<GetRolePermessionsQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetRolePermessionsHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }
    public async Task<IResponseWrapper> Handle(GetRolePermessionsQuery request, CancellationToken cancellationToken)
    {
        return await _roleService.GetPermessionsAsync(request.RoleId);
    }
}
