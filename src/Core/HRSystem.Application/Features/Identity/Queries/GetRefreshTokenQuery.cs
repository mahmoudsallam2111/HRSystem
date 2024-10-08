using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Identity.Queries;

public class GetRefreshTokenQuery : IRequest<IResponseWrapper>
{
    public RefreshTokenRequest refreshTokenRequest { get; set; }
}


public class GetRefreshTokenHandler(ITokenService tokenService) :
    IRequestHandler<GetRefreshTokenQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        return await tokenService.GetRefreshTokenAsync(request.refreshTokenRequest);
    }
}