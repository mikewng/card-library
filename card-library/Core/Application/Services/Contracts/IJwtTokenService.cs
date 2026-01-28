using card_library.Core.Application.Models;

namespace card_library.Core.Application.Services.Contracts
{
    public interface IJwtTokenService
    {
        (string token, DateTime expiresAtUtc) CreateAccessToken(User user);
    }
}
