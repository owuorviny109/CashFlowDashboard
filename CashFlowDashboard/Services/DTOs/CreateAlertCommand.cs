using CashFlowDashboard.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CashFlowDashboard.Services.DTOs;

public record CreateAlertCommand
{
    [Required]
    public AlertSeverity Severity { get; init; }

    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Message { get; init; } = string.Empty;

    [Required]
    public AlertCategory Category { get; init; }
}
