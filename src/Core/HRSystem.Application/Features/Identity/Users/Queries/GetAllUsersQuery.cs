using HRSystem.Application.Services.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Users.Queries;

public class GetAllUsersQuery : IRequest<IResponseWrapper>
{
}


public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetAllUsersHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetAllUsersAsync();
    }
}
