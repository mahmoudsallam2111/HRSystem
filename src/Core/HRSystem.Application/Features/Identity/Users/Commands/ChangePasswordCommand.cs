using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Users.Commands;

public class ChangePasswordCommand : IRequest<IResponseWrapper>
{
    public ChangePasswordRequest ChangePasswordRequest { get; set; }
}


public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public ChangePasswordHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return await _userService.ChangeUserPasswordAsync(request.ChangePasswordRequest);
    }
}