using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Roles.Commands;

public class UpdateRolePermessionCommand : IRequest<IResponseWrapper>
{
    public UpdateRoleClaimsRequest updateRoleClaimsRequest { get; set; }
}


public class UpdateRolePermessionHandler(IRoleService _roleService)
    : IRequestHandler<UpdateRolePermessionCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateRolePermessionCommand request, CancellationToken cancellationToken)
    {
        return await _roleService.UpdateRolePermessionsAsync(request.updateRoleClaimsRequest);
    }
}
