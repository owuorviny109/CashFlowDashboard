namespace CashFlowDashboard.Services.DTOs;

public record ForecastDataPointDto(
    DateOnly Date,
    decimal ProjectedBalance,
    decimal LowerBound,
    decimal UpperBound
);
