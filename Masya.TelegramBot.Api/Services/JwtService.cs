using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Masya.TelegramBot.Api.Options;
using Masya.TelegramBot.DataAccess.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Masya.TelegramBot.Api.Services
{
    public sealed class JwtService : IJwtService
    {
        public JwtOptions Options { get; }

        public JwtService(IOptions<JwtOptions> options)
        {
            Options = options.Value;
        }

        private string GenerateToken(Claim[] claims, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = Options.Issuer,
                Audience = Options.Audience,
                SigningCredentials = new SigningCredentials(Options.SecurityKey, SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateAccessToken(User user)
        {
            Claim[] claims = new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, user.TelegramAccountId.ToString())
            };

            return GenerateToken(claims, DateTime.UtcNow.AddMinutes(Options.ExpiresInMinutes));
        }

        public string GenerateRefreshToken(User user)
        {
            Claim[] claims = new Claim[] {
                new Claim(ClaimTypes.Name, user.TelegramLogin)
            };

            return GenerateToken(claims, DateTime.UtcNow.AddDays(Options.RefreshExpiresInDays));
        }

        public IEnumerable<Claim> GetClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token);

            return securityToken.Claims;
        }
    }
}