using HRSystem.Application.Services.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Users.Queries;

public class GetUserByIdQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}



public class GetUserByIdHandler(IUserService userService)
    : IRequestHandler<GetUserByIdQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetUserByIdAsync(request.UserId);
    }
}

