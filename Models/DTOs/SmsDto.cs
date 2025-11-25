namespace MarketingAPI.Models.DTOs;

public class SmsDto
{
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}