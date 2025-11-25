using MarketingAPI.Data;
using MarketingAPI.Models;
using MarketingAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MarketingAPI.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
/// <param name="configuration"></param>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SmsController(ApplicationDbContext context, ILogger<UsersController> logger, IConfiguration configuration) : ControllerBase
{
    string? accountSsid;
    string? authToken;
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<UsersController> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Sends a message to a single recipient.
    /// </summary>
    /// <param name="request">The SMS request object.</param>
    /// <returns>Object code result that corresponds to the code send to the client.</returns>
    [HttpPost("send")]
    public async Task<IActionResult> SendSms([FromBody] SmsDto request)
    {
        try
        {
            string? accountSid = _configuration["Twilio:AccountSid"];
            string? authToken = _configuration["Twilio:AuthToken"];
            string? fromNumber = _configuration["Twilio:PhoneNumber"];

            TwilioClient.Init(accountSid, authToken);

            MessageResource message = await MessageResource.CreateAsync(
                to: new PhoneNumber(request.Recipient),
                from: new PhoneNumber(fromNumber),
                body: request.Message
            );

            _logger.LogInformation($"SMS sent successfully. SID: {message.Sid}");

            return Ok(new
            {
                success = true,
                messageSid = message.Sid,
                status = message.Status.ToString()
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send message");
            return BadRequest(new { success = false, error = e.Message });
        }
    }

    /// <summary>
    /// Sends bulk messages to a group of people based on the DTO tag received.
    /// </summary>
    /// <param name="dto">The DTO object send with the request.</param>
    /// <returns></returns>
    [HttpPost("bulk")]
    public async Task<IActionResult> SendBulkSms([FromBody] BulkSmsDto dto)
    {
        string type = dto.RecipientTag.Split(':')[0];
        string evt = dto.RecipientTag.Split(':')[1];

        List<Customer> customers = type switch
        {
            "event" => [.. _context.Customers.Where(c => c.Interest == evt)],
            "date" => [.. _context.Customers.Where(c => false)],
            _ => [.. _context.Customers]
        };

        if (customers.Count == 0)
            return BadRequest($"There are no customers that match the tag {dto.RecipientTag}");

        try
        {
            string? accountSid = _configuration["Twilio:AccountSid"];
            string? authToken = _configuration["Twilio:AuthToken"];
            string? messagingServiceSid = _configuration["Twilio:MessagingServiceSid"];

            TwilioClient.Init(accountSid, authToken);
            List<MessageResult> results = [];
            foreach (Customer customer in customers)
            {
                try
                {
                    MessageResource message = await MessageResource.CreateAsync(
                        body: ResolveTemplate(dto.MessageTemplate, customer),
                        messagingServiceSid: messagingServiceSid,
                        to: new PhoneNumber(customer.PhoneNumber)
                    );

                    results.Add(new()
                    {
                        PhoneNumber = customer.PhoneNumber,
                        MessageSid = message.Sid,
                        Status = message.Status.ToString(),
                        Success = true
                    });

                    _logger.LogInformation($"Message sent to {customer.PhoneNumber}: {message.Sid}");
                }
                catch (Exception e)
                {
                    results.Add(new MessageResult
                    {
                        PhoneNumber = customer.PhoneNumber,
                        Success = false,
                        Error = e.Message
                    });

                    _logger.LogError(e, $"Failed to send message to {customer.PhoneNumber}");
                }
            }

            return Ok(new
            {
                totalRecipients = customers.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                results
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Bulk SMS Operation failed.");
            return BadRequest(new { error = e.Message });
        }
    }
    
    private string ResolveTemplate(string template, Customer customer)
    {
        return template.Replace("{name}", customer.Name);
    }
}

/// <summary>
/// Results of a message being sent
/// </summary>
public struct MessageResult
{
    public string PhoneNumber { get; set; }
    public string MessageSid { get; set; }
    public string Status { get; set; }
    public bool Success { get; set; }
    public string Error { get; set; }
}
