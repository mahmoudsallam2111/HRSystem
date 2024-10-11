using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Users.Commands;

public class UserRegistrationCommand : IRequest<IResponseWrapper>
{
    public UserRegistrationRequest UserRegistrationRequest { get; set; }
}


public class UserRegistrationHandler : IRequestHandler<UserRegistrationCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public UserRegistrationHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
    {
        return await _userService.RegisterUserAsync(request.UserRegistrationRequest);
    }
}
