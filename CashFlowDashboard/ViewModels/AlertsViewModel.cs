using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.ViewModels;

public sealed class AlertsViewModel
{
    // Grouped by time for better UX
    public IReadOnlyList<AlertDto> TodayAlerts { get; init; } = Array.Empty<AlertDto>();
    public IReadOnlyList<AlertDto> YesterdayAlerts { get; init; } = Array.Empty<AlertDto>();
    public IReadOnlyList<AlertDto> OlderAlerts { get; init; } = Array.Empty<AlertDto>();
    
    public int UnreadCount { get; init; }
    public string CurrentFilter { get; init; } = "All"; // "All", "Critical", "Warning"

    // Pagination
    public int TotalCount { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
