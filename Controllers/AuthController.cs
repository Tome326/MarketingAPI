using MarketingAPI.Data;
using MarketingAPI.Models;
using MarketingAPI.Models.DTOs;
using MarketingAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ApplicationDbContext context, IPasswordService passwordService, ITokenService tokenService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            return BadRequest(new { message = "Username already exists." });

        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            return BadRequest(new { message = "Email already in use" });

        User user = new()
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        user.PasswordHash = _passwordService.HashPassword(user, registerDto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"User {user.Username} register successfully.");

        UserDto userDto = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
        };

        return CreatedAtAction(nameof(Register), new { id = user.Id }, userDto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user is null)
            return Unauthorized(new { message = "Invalid username or password." });

        if (!user.IsActive)
            return Unauthorized(new { message = "Account is inactive" });

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        string token = _tokenService.GenerateToken(user);

        _logger.LogInformation($"User logged in successfully: {user.Username}");

        return Ok(new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email
        });
    }
}