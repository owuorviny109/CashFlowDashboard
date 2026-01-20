namespace CashFlowDashboard.Models.Enums;

// Defines the severity level of system alerts.
// Ordered by priority: Critical > Warning > Info > Success.
public enum AlertSeverity
{
    // Critical alert requiring immediate attention (e.g., low balance, overdue invoice).
    Critical = 0,

    // Warning alert indicating potential issues (e.g., projected shortfall).
    Warning = 1,

    // Informational alert for awareness (e.g., large transaction detected).
    Info = 2,

    // Success notification for positive events (e.g., goal achieved).
    Success = 3
}
