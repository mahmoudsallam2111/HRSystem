using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests;
using HRSystem.Common.Responses;

namespace HRSystem.Infrastructure.Persistence.Services.Identity
{
    public class TokenService : ITokenService
    {
        public Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest)
        {
            throw new NotImplementedException();
        }
        public Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            throw new NotImplementedException();
        }

    }
}
