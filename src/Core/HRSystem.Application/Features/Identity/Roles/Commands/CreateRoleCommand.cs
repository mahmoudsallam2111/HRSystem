using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Roles.Commands;

public class CreateRoleCommand : IRequest<IResponseWrapper>
{
    public CreateRoleRequest CreateRoleRequest { get; set; }
}


public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public CreateRoleHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }
    public Task<IResponseWrapper> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        return _roleService.CreateRoleAsync(request.CreateRoleRequest);
    }
}