using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CashFlowDashboard.Data.Repositories;

// Implementation of IAlertRepository using EF Core.
public class AlertRepository : IAlertRepository
{
    private readonly CashFlowDbContext _context;

    public AlertRepository(CashFlowDbContext context)
    {
        _context = context;
    }

    // Retrieves a single alert by its unique identifier.
    public async Task<Alert?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Alerts.FindAsync(new object[] { id }, ct);
    }

    // Retrieves all active alerts (not resolved or dismissed).
    public async Task<IReadOnlyList<Alert>> GetActiveAlertsAsync(CancellationToken ct = default)
    {
        return await _context.Alerts
            .AsNoTracking()
            .Where(a => a.Status != AlertStatus.Resolved && a.Status != AlertStatus.Dismissed)
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync(ct);
    }

    // Retrieves alerts filtered by specific severity level.
    public async Task<IReadOnlyList<Alert>> GetBySeverityAsync(AlertSeverity severity, CancellationToken ct = default)
    {
        return await _context.Alerts
            .AsNoTracking()
            .Where(a => a.Severity == severity)
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync(ct);
    }

    // Retrieves all alerts strictly with 'Unread' status.
    public async Task<IReadOnlyList<Alert>> GetUnreadAsync(CancellationToken ct = default)
    {
        return await _context.Alerts
            .AsNoTracking()
            .Where(a => a.Status == AlertStatus.Unread)
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync(ct);
    }

    // Adds a new alert to the database.
    public async Task AddAsync(Alert alert, CancellationToken ct = default)
    {
        await _context.Alerts.AddAsync(alert, ct);
        await _context.SaveChangesAsync(ct);
    }

    // Updates the status of a specific alert.
    public async Task UpdateStatusAsync(Guid id, AlertStatus newStatus, CancellationToken ct = default)
    {
        var alert = await _context.Alerts.FindAsync(new object[] { id }, ct);
        if (alert != null)
        {
            alert.Status = newStatus;
            await _context.SaveChangesAsync(ct);
        }
    }

    // Bulk updates all 'Unread' alerts to 'Read'.
    // Uses ExecuteUpdateAsync for performance (avoids loading entities).
    public async Task MarkAllAsReadAsync(CancellationToken ct = default)
    {
        await _context.Alerts
            .Where(a => a.Status == AlertStatus.Unread)
            .ExecuteUpdateAsync(calls => calls.SetProperty(a => a.Status, AlertStatus.Read), ct);
    }

    // Removes or archives alerts older than a specific date.
    public async Task ArchiveOldAlertsAsync(DateTime olderThan, CancellationToken ct = default)
    {
        await _context.Alerts
            .Where(a => a.GeneratedAt < olderThan)
            .ExecuteDeleteAsync(ct);
    }
}
