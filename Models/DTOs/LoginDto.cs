using System.ComponentModel.DataAnnotations;

namespace MarketingAPI.Models.DTOs;

/// <summary>
/// Data transfer object for the Login request
/// </summary>
public class LoginDto
{
    /// <summary>
    /// The username to use to attempt to login.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password to use to attempt to login.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}