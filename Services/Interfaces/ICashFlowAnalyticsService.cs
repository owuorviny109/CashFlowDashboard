using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.Services.Interfaces;

public interface ICashFlowAnalyticsService
{
    Task<CashFlowTrendDto> GetTrendForPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default);
    Task<decimal> GetCurrentBalanceAsync(CancellationToken ct = default);
    Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct = default);
    Task<GrowthMetricsDto> GetGrowthMetricsAsync(CancellationToken ct = default);
    Task GenerateDailySnapshotAsync(DateOnly date, CancellationToken ct = default);
}
