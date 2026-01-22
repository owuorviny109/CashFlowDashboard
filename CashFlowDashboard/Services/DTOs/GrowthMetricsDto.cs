namespace CashFlowDashboard.Services.DTOs;

public record GrowthMetricsDto(
    decimal CurrentBalance,
    decimal BalanceChangePercent,
    decimal NetCashFlow30Day,
    decimal BurnRate
);
