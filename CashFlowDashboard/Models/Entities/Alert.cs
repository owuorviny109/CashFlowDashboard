using System.ComponentModel.DataAnnotations;
using CashFlowDashboard.Models.Enums;

namespace CashFlowDashboard.Models.Entities;

// Represents a system-generated or manual alert/notification.
// Alerts have lifecycle states and can be triggered by business rules.
public sealed class Alert
{
    // Unique identifier for the alert.
    [Key]
    public Guid Id { get; set; }

    // Priority level of the alert.
    // Ordered: Critical > Warning > Info > Success
    [Required]
    public AlertSeverity Severity { get; set; }

    // Short, actionable headline for the alert.
    // Displayed in notification lists and dashboards.
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    // Detailed explanation of the alert and recommended actions.
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;

    // Functional domain category for filtering and organization.
    [Required]
    public AlertCategory Category { get; set; }

    // Current lifecycle status of the alert.
    [Required]
    public AlertStatus Status { get; set; }

    // Timestamp when the alert was generated.
    // Indexed for efficient time-based queries.
    [Required]
    public DateTime GeneratedAt { get; set; }

    // Identifier of the rule or user action that created this alert.
    // Examples: "LowBalanceRule", "InvoiceOverdueRule", "ManualAlert"
    [StringLength(100)]
    public string? TriggeredBy { get; set; }

    // Foreign key reference to related entity (Transaction, Forecast, etc.).
    // Null for system-wide alerts.
    public Guid? RelatedEntityId { get; set; }

    // Deep link URL to the related page or action.
    // Example: "/Transactions/Details/{id}"
    [StringLength(500)]
    public string? ActionUrl { get; set; }

    // Optional timestamp for automatic dismissal/archival.
    // Null for alerts that don't expire.
    public DateTime? ExpiresAt { get; set; }
}
