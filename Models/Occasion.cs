using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingAPI.Models;

/// <summary>
/// Occasion model
/// </summary>
public class Occasion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OccasionName { get; set; } = string.Empty;

    [Required]
    public string OccasionType { get; set; } = string.Empty;


    public List<string> OccasionAttendees { get; set; } = [];


    [Required]
    public DateTime OccasionDateTime { get; set; } = DateTime.UtcNow;
    
    public string OccasionDuration { get; set; } = string.Empty;
}