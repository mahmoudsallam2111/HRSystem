using HRSystem.Common.Requests;
using HRSystem.Common.Responses;

namespace HRSystem.Application.Services.Identity
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest);
        Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    }
}
