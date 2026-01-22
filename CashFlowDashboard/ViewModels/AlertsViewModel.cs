using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.ViewModels;

public sealed class AlertsViewModel
{
    // Grouped by time for better UX
    public IReadOnlyList<AlertDto> TodayAlerts { get; init; } = Array.Empty<AlertDto>();
    public IReadOnlyList<AlertDto> YesterdayAlerts { get; init; } = Array.Empty<AlertDto>();
    public IReadOnlyList<AlertDto> OlderAlerts { get; init; } = Array.Empty<AlertDto>();
    
    public int UnreadCount { get; init; }
}
