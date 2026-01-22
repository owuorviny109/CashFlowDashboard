using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.Services.Interfaces;

public interface IAlertService
{
    Task<AlertDto> CreateManualAlertAsync(CreateAlertCommand cmd, CancellationToken ct = default);
    Task<IReadOnlyList<AlertDto>> GetActiveAlertsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<AlertDto>> GetAlertsBySeverityAsync(AlertSeverity severity, CancellationToken ct = default);
    Task MarkAsReadAsync(Guid alertId, CancellationToken ct = default);
    Task DismissAlertAsync(Guid alertId, CancellationToken ct = default);
    Task MarkAllAsReadAsync(CancellationToken ct = default);
    Task GenerateSystemAlertsAsync(CancellationToken ct = default);
}
