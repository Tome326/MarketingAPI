using MarketingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace MarketingAPI.Services;

public class PasswordService(IPasswordHasher<User> passwordHasher) : IPasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public PasswordVerificationResult VerifyPassword(User user, string password, string passwordHash)
    {
        return _passwordHasher.VerifyHashedPassword(user, passwordHash, password);
    }
}