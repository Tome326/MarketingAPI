using MarketingAPI.Models;

namespace MarketingAPI.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}