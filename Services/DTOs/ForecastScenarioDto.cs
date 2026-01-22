using CashFlowDashboard.Models.Enums;

namespace CashFlowDashboard.Services.DTOs;

public record ForecastScenarioDto(
    Guid Id,
    string Name,
    ScenarioType ScenarioType,
    List<ForecastDataPointDto> DataPoints,
    decimal ConfidenceLevel,
    decimal EndCashBalance
);
