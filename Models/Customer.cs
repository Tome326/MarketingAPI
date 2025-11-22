using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingAPI.Models;

/// <summary>
/// Customer Model
/// </summary>
public class Customer
{
    /// <summary>
    /// Customer ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Customer Name
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Customer Email
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer Phone Number
    /// </summary>
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer Birthday
    /// </summary>
    [Required]
    public DateTime Birthday { get; set; }

    /// <summary>
    /// Customer Main Interest
    /// </summary>
    [Required]
    public string Interest { get; set; } = string.Empty;

    /// <summary>
    /// Customer SMS agreement
    /// </summary>
    [Required]
    public bool AgreeToSms { get; set; } = false;
}