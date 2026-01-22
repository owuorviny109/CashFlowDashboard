using CashFlowDashboard.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CashFlowDashboard.Services.DTOs;

public record CreateTransactionCommand
{
    [Required]
    public DateTime Date { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; init; }

    [Required]
    public TransactionType Type { get; init; }

    [Required]
    [StringLength(100)]
    public string Category { get; init; } = string.Empty;

    [StringLength(500)]
    public string Description { get; init; } = string.Empty;

    public bool IsRecurring { get; init; }

    public RecurrencePattern? RecurrencePattern { get; init; }
}
