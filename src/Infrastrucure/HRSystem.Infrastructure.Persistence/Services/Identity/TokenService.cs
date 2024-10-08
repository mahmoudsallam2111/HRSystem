﻿using HRSystem.Application.AppConfigs;
using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Identity;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HRSystem.Infrastructure.Persistence.Services.Identity
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppConfiguration _appConfiguration;

        public TokenService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<AppConfiguration> appConfiguration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appConfiguration = appConfiguration.Value;
        }
        public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
        {
            // validate user
            var user  = await _userManager.FindByEmailAsync(tokenRequest.Email);
            if (user == null)
                return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials");

            // check if user active
            if (!user.IsActive)
                return await ResponseWrapper<TokenResponse>.FailAsync("user is inactive. please contact the adminastrator");

            // check if email confirmed
            if (!user.EmailConfirmed)
                return await ResponseWrapper<TokenResponse>.FailAsync("Email is Not Confirmed");

            // check password
            var isPasswordConfirmed = await _userManager.CheckPasswordAsync(user,tokenRequest.Password);
            if (!isPasswordConfirmed)
                return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials");

            // generate refresh token
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);

            // update user
            await _userManager.UpdateAsync(user);  // cause refresh token is stored in the database

            //gentrate new token 
            var token = await GenerateJWTTokenAsync(user);
            var reponse = new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpoiryTime = user.RefreshTokenExpiryDate.Value
            };

            return await ResponseWrapper<TokenResponse>.SuccessAsync(reponse);
        }
        public async Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token.");
            }
            var userPrincipal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user is null)
                return await ResponseWrapper<TokenResponse>.FailAsync("User Not Found.");
            if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryDate <= DateTime.Now)
                return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token.");

            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpoiryTime = user.RefreshTokenExpiryDate.Value
            };
            return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
        }


  #region private methods
        private string GenerateRefreshToken()
        {
            var bytesNumber = new byte[32]; 
            using var rnd =  RandomNumberGenerator.Create();
            rnd.GetBytes(bytesNumber);

            return Convert.ToBase64String(bytesNumber); 
        }

        private async Task<string> GenerateJWTTokenAsync(ApplicationUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials , IEnumerable<Claim> claims)
        {
            var token =  new JwtSecurityToken(
               claims:claims,
               expires:DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
               signingCredentials: signingCredentials);
            // create handler
            var tokenHandler = new JwtSecurityTokenHandler();   

            var encryptedToken = tokenHandler.WriteToken(token);   

            return encryptedToken;

        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret);

            return new SigningCredentials(new SymmetricSecurityKey(secret),SecurityAlgorithms.HmacSha256);
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user) 
        { 
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user); 
            var roleClaims = new List<Claim>();
            var permessionClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var currentRole = await _roleManager.FindByNameAsync(role);
                var allPermessionForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
                permessionClaims.AddRange(allPermessionForCurrentRole); 
            }

            // construct Claim Object
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.Name , user.FirstName),
                new Claim(ClaimTypes.Surname , user.LastName),
                new Claim(ClaimTypes.MobilePhone , user.PhoneNumber ?? string.Empty),

            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permessionClaims);

            return claims;
        }


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

 #endregion

    }
}
