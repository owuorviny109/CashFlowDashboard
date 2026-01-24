using CashFlowDashboard.Configuration;
using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Services.DTOs;
using CashFlowDashboard.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CashFlowDashboard.Services;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IForecastRepository _forecastRepository;
    private readonly AlertSettings _settings;
    private readonly ILogger<AlertService> _logger;

    public AlertService(
        IAlertRepository alertRepository,
        ITransactionRepository transactionRepository,
        IForecastRepository forecastRepository,
        IOptions<AlertSettings> settings,
        ILogger<AlertService> logger)
    {
        _alertRepository = alertRepository;
        _transactionRepository = transactionRepository;
        _forecastRepository = forecastRepository;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<AlertDto> CreateManualAlertAsync(CreateAlertCommand cmd, CancellationToken ct = default)
    {
        var alert = new Alert
        {
            Id = Guid.NewGuid(),
            Severity = cmd.Severity,
            Title = cmd.Title,
            Message = cmd.Message,
            Category = cmd.Category,
            Status = AlertStatus.Unread,
            GeneratedAt = DateTime.UtcNow
        };

        await _alertRepository.AddAsync(alert, ct);
        return ToDto(alert);
    }

    public async Task<IReadOnlyList<AlertDto>> GetActiveAlertsAsync(CancellationToken ct = default)
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync(ct);
        return alerts.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<AlertDto>> GetAlertsBySeverityAsync(AlertSeverity severity, CancellationToken ct = default)
    {
        var alerts = await _alertRepository.GetBySeverityAsync(severity, ct);
        return alerts.Select(ToDto).ToList();
    }

    public async Task MarkAsReadAsync(Guid alertId, CancellationToken ct = default)
    {
        await _alertRepository.UpdateStatusAsync(alertId, AlertStatus.Read, ct);
    }

    public async Task DismissAlertAsync(Guid alertId, CancellationToken ct = default)
    {
        await _alertRepository.UpdateStatusAsync(alertId, AlertStatus.Dismissed, ct);
    }

    public async Task MarkAllAsReadAsync(CancellationToken ct = default)
    {
        await _alertRepository.MarkAllAsReadAsync(ct);
    }

    public async Task GenerateSystemAlertsAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting system alert generation...");

        var newAlerts = new List<Alert>();

        // Rule 1: Low Balance Check
        // Intent: Prevent overdraft fees/operations stoppage.
        var currentBalance = await _transactionRepository.GetBalanceAtDateAsync(DateTime.Today, ct);
        if (currentBalance < _settings.LowBalanceThreshold)
        {
            newAlerts.Add(new Alert
            {
                Id = Guid.NewGuid(),
                Severity = AlertSeverity.Critical,
                Category = AlertCategory.CashFlow,
                Title = "Low Balance Warning",
                Message = $"Current balance ({currentBalance:C0}) is below the threshold of {_settings.LowBalanceThreshold:C0}.",
                Status = AlertStatus.Unread,
                GeneratedAt = DateTime.UtcNow,
                TriggeredBy = "Rule:LowBalance"
            });
        }

        // Rule 2: Large Transaction Check (Last 24 hours)
        // Assumption: Job runs daily. We check recent txns to catch outliers.
        // TODO: Replace with "Since Last Checked" logic for true exactly-once alerting.
        var recentTxns = await _transactionRepository.GetRecentAsync(50, ct); 
        
        foreach (var txn in recentTxns.Where(t => t.Date.Date == DateTime.Today && t.Amount >= _settings.LargeTransactionThreshold))
        {
             // TriggeredBy uses TxnId to ensure we don't alert twice for the same transaction
            newAlerts.Add(new Alert
            {
                Id = Guid.NewGuid(),
                Severity = AlertSeverity.Info,
                Category = AlertCategory.CashFlow,
                Title = "Large Transaction Detected",
                Message = $"A large transaction of {txn.Amount:C0} was recorded on {txn.Date:d}.",
                Status = AlertStatus.Unread,
                GeneratedAt = DateTime.UtcNow,
                TriggeredBy = $"Rule:LargeTxn:{txn.Id}",
                RelatedEntityId = txn.Id
            });
        }
        
        // Rule 3: Projected Shortfall (Forecast)
        // Intent: Give advanced warning (up to 30 days) of potential negative balance.
        var forecasts = await _forecastRepository.GetActiveScenariosAsync(ct);
        var baseCase = forecasts.FirstOrDefault(f => f.ScenarioType == ScenarioType.BaseCase);
        
        if (baseCase != null)
        {
            var shortfallPoint = baseCase.DataPoints.FirstOrDefault(dp => dp.ProjectedBalance < 0);
            if (shortfallPoint != null)
            {
                 newAlerts.Add(new Alert
                {
                    Id = Guid.NewGuid(),
                    Severity = AlertSeverity.Warning,
                    Category = AlertCategory.Forecast,
                    Title = "Projected Cash Shortfall",
                    Message = $"Forecast predicts a negative balance on {shortfallPoint.Date:d}.",
                    Status = AlertStatus.Unread,
                    GeneratedAt = DateTime.UtcNow,
                    TriggeredBy = $"Rule:Shortfall:{baseCase.Id}"
                });
            }
        }

        // Batch Save with Deduplication
        // Reason: Prevent spam creation if the job runs multiple times a day.
        var activeAlerts = await _alertRepository.GetActiveAlertsAsync(ct);
        
        foreach (var alert in newAlerts)
        {
            // Fix: Check for ANY alert with this trigger today, regadless of Status (Read or Unread).
            // This prevents re-alerting on issues the user has already acknowledged.
            if (!activeAlerts.Any(a => a.TriggeredBy == alert.TriggeredBy && a.GeneratedAt.Date == DateTime.Today))
            {
                await _alertRepository.AddAsync(alert, ct);
                 _logger.LogInformation("Generated alert: {Title}", alert.Title);
            }
        }
        
        _logger.LogInformation("Completed system alert generation.");
    }

    private static AlertDto ToDto(Alert alert)
    {
        // Simple humanizing logic for time
        var span = DateTime.UtcNow - alert.GeneratedAt;
        string timeAgo = span.TotalHours < 1 ? $"{span.Minutes}m ago" : 
                         span.TotalHours < 24 ? $"{span.Hours}h ago" : 
                         $"{span.Days}d ago";

        return new AlertDto(
            Id: alert.Id,
            Severity: alert.Severity,
            Title: alert.Title,
            Message: alert.Message,
            TimeAgo: timeAgo,
            Status: alert.Status,
            ActionUrl: alert.ActionUrl,
            GeneratedAt: alert.GeneratedAt
        );
    }
}
