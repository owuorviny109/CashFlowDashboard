using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.Services.Interfaces;

public interface IAlertService
{
    /// <summary>
    /// Creates a manual system alert (e.g. for testing or external integrations).
    /// </summary>
    Task<AlertDto> CreateManualAlertAsync(CreateAlertCommand cmd, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all active alerts (not resolved or dismissed).
    /// </summary>
    Task<IReadOnlyList<AlertDto>> GetActiveAlertsAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves alerts filtered by specific severity level.
    /// </summary>
    Task<IReadOnlyList<AlertDto>> GetAlertsBySeverityAsync(AlertSeverity severity, CancellationToken ct = default);

    /// <summary>
    /// Marks a specific alert as read.
    /// </summary>
    Task MarkAsReadAsync(Guid alertId, CancellationToken ct = default);

    /// <summary>
    /// Dismisses an alert (hides it from the list).
    /// </summary>
    Task DismissAlertAsync(Guid alertId, CancellationToken ct = default);

    /// <summary>
    /// Marks all unread alerts as read.
    /// </summary>
    Task MarkAllAsReadAsync(CancellationToken ct = default);

    /// <summary>
    /// Triggers the system alert generation logic (checks rules against current data).
    /// </summary>
    Task GenerateSystemAlertsAsync(CancellationToken ct = default);
}
