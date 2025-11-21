using MarketingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace MarketingAPI.Services;

public interface IPasswordService
{
    public string HashPassword(User user, string password);
    public PasswordVerificationResult VerifyPassword(User user, string password, string passwordHash);
}