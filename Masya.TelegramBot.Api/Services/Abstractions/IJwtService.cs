using System.Security.Claims;
using Masya.TelegramBot.Api.Options;
using Masya.TelegramBot.DataAccess.Models;

namespace Masya.TelegramBot.Api.Services.Abstractions
{
  public interface IJwtService
  {
    JwtOptions Options { get; }
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    ClaimsPrincipal Validate(string token);
  }
}