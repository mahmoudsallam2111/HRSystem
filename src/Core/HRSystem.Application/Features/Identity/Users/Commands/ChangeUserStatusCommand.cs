using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Users.Commands;

public class ChangeUserStatusCommand : IRequest<IResponseWrapper>
{
    public ChangeUserStatusRequest ChangeUserStatusRequest { get; set; }
}

public class ChangeUserStatusHandler(IUserService userService) :
    IRequestHandler<ChangeUserStatusCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
    {
        return await userService.ChangeUserStatusAsync(request.ChangeUserStatusRequest);
    }
}
