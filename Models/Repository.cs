using System.ComponentModel.DataAnnotations;

namespace MetricsAPI.Models;

public class Repository
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Language { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}