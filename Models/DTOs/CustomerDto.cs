using System.ComponentModel.DataAnnotations;

namespace MarketingAPI.Models.DTOs;

public class CustomerDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    public string Interest { get; set; } = string.Empty;
    public bool AgreeToSms { get; set; } = false;
}