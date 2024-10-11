using HRSystem.Application.Services.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Users.Queries;

public class GetUserRolesQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}


public class GetUserRolesHandler(IUserService userService)
    : IRequestHandler<GetUserRolesQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetRolesAsyn(request.UserId);
    }
}