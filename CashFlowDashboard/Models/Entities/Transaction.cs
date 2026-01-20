using System.ComponentModel.DataAnnotations;
using CashFlowDashboard.Models.Enums;

namespace CashFlowDashboard.Models.Entities;

// Represents a single cash flow transaction (income or expense).
// Immutable after creation for historical transactions (older than 90 days).
public sealed class Transaction
{
    // Unique identifier for the transaction.
    [Key]
    public Guid Id { get; set; }

    // Date and time when the transaction occurred.
    // Indexed for efficient date-range queries.
    [Required]
    public DateTime Date { get; set; }

    // Monetary amount of the transaction.
    // Must be positive; direction is determined by Type property.
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    // Direction of cash flow (Income or Expense).
    [Required]
    public TransactionType Type { get; set; }

    // Business category for classification and reporting.
    // Examples: "Sales", "Software", "Operations", "Payroll"
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Category { get; set; } = string.Empty;

    // Human-readable description of the transaction.
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;

    // External reference identifier (e.g., invoice number, receipt ID).
    // Optional for manual entries.
    [StringLength(100)]
    public string? ReferenceId { get; set; }

    // Indicates whether this transaction repeats on a schedule.
    public bool IsRecurring { get; set; }

    // Frequency pattern for recurring transactions.
    // Null if IsRecurring is false.
    public RecurrencePattern? RecurrencePattern { get; set; }

    // Audit timestamp: when this record was created.
    [Required]
    public DateTime CreatedAt { get; set; }

    // Audit timestamp: when this record was last modified.
    // Null if never modified after creation.
    public DateTime? ModifiedAt { get; set; }
}
