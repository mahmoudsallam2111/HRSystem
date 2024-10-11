using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Users.Commands;

public class UpdateUserCommand : IRequest<IResponseWrapper>
{
    public UpdateUserRequest UpdateUserRequest { get; set; }
}



public class UpdateUserHandler(IUserService userService)
    : IRequestHandler<UpdateUserCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await userService.UpdateUserAsync(request.UpdateUserRequest);
    }
}