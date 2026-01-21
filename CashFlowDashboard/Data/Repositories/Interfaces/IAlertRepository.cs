using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;

namespace CashFlowDashboard.Data.Repositories.Interfaces;

// Interface for Alert data access.
// Manages retrieval and lifecycle state updates for system notification.
public interface IAlertRepository
{
    // Retrieves a single alert by its unique identifier.
    Task<Alert?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Retrieves all active alerts (not resolved or dismissed).
    // Ordered by generation date descending (newest first).
    Task<IReadOnlyList<Alert>> GetActiveAlertsAsync(CancellationToken ct = default);

    // Retrieves alerts filtered by specific severity level.
    // Examples: Get all Critical alerts, or all Warnings.
    Task<IReadOnlyList<Alert>> GetBySeverityAsync(AlertSeverity severity, CancellationToken ct = default);

    // Retrieves all alerts strictly with 'Unread' status.
    Task<IReadOnlyList<Alert>> GetUnreadAsync(CancellationToken ct = default);

    // Adds a new alert to the database.
    Task AddAsync(Alert alert, CancellationToken ct = default);

    // Updates the status of a specific alert (e.g. Unread -> Read, or -> Dismissed).
    Task UpdateStatusAsync(Guid id, AlertStatus newStatus, CancellationToken ct = default);

    // Bulk updates all 'Unread' alerts to 'Read'.
    // Typical "Mark All as Read" feature.
    Task MarkAllAsReadAsync(CancellationToken ct = default);

    // Removes or archives alerts older than a specific date to maintain table size.
    Task ArchiveOldAlertsAsync(DateTime olderThan, CancellationToken ct = default);
}
