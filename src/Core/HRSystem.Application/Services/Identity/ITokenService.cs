using HRSystem.Common.Requests;
using HRSystem.Common.Responses;
using HRSystem.Common.Responses.Wrapper;

namespace HRSystem.Application.Services.Identity
{
    public interface ITokenService
    {
        Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest);
        Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    }
}
