using System.ComponentModel.DataAnnotations;

namespace CashFlowDashboard.Services.DTOs;

public record UpdateTransactionCommand
{
    [Range(0.01, double.MaxValue)]
    public decimal? Amount { get; init; }

    [StringLength(100)]
    public string? Category { get; init; }

    [StringLength(500)]
    public string? Description { get; init; }
}
