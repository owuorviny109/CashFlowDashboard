using System.ComponentModel.DataAnnotations;

namespace CashFlowDashboard.Models;

public class AppSetting
{
    [Key]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;
    
    // Helper to store Type info if needed (string, decimal, int)
    [MaxLength(50)]
    public string DataType { get; set; } = "string";
}
