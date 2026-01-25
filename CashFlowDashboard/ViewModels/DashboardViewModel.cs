using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.ViewModels;

public sealed class DashboardViewModel
{
    // KPI Metrics
    public decimal CurrentBalance { get; init; }
    public decimal BalanceChangePercent { get; init; }
    public decimal NetCashFlow30Day { get; init; }
    public decimal ForecastGrowth60Day { get; init; }
    public int ActiveAlertCount { get; init; }

    // Chart Data
    public IReadOnlyList<ChartDataPoint> HistoricalChartData { get; init; } = Array.Empty<ChartDataPoint>();
    public IReadOnlyList<ChartDataPoint> ProjectedChartData { get; init; } = Array.Empty<ChartDataPoint>();

    // Recent Activity
    public string SelectedRange { get; init; } = "6M";
    public IReadOnlyList<AlertDto> RecentAlerts { get; init; } = Array.Empty<AlertDto>();
    public IReadOnlyList<TransactionDto> RecentTransactions { get; init; } = Array.Empty<TransactionDto>();
}
