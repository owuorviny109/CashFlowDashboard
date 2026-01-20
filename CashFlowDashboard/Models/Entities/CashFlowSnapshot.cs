using System.ComponentModel.DataAnnotations;

namespace CashFlowDashboard.Models.Entities;

// Immutable point-in-time snapshot of account state for a specific date.
// Generated daily via background job for historical trend analysis.
public sealed class CashFlowSnapshot
{
    // Unique identifier for the snapshot.
    [Key]
    public Guid Id { get; set; }

    // Date this snapshot represents.
    // Must be unique (one snapshot per day).
    [Required]
    public DateOnly Date { get; set; }

    // Cash balance at the start of the day.
    // Equals the closing balance of the previous day.
    [Required]
    public decimal OpeningBalance { get; set; }

    // Sum of all income transactions for this date.
    [Required]
    public decimal TotalIncome { get; set; }

    // Sum of all expense transactions for this date.
    [Required]
    public decimal TotalExpenses { get; set; }

    // Cash balance at the end of the day.
    // Computed: OpeningBalance + TotalIncome - TotalExpenses
    [Required]
    public decimal ClosingBalance { get; set; }

    // Net cash flow for the day.
    // Computed: TotalIncome - TotalExpenses
    [Required]
    public decimal NetCashFlow { get; set; }

    // Number of transactions processed on this date.
    [Required]
    public int TransactionCount { get; set; }

    // Timestamp when this snapshot was computed.
    [Required]
    public DateTime ComputedAt { get; set; }
}
