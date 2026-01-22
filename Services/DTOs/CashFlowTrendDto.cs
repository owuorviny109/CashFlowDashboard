using CashFlowDashboard.Models.Enums;

namespace CashFlowDashboard.Services.DTOs;

public record CashFlowTrendDto(
    List<ChartDataPoint> DataPoints,
    TrendDirection Direction,
    decimal GrowthRate
);
