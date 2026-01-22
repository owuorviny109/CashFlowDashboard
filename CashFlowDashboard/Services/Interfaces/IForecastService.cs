using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.Services.Interfaces;

public interface IForecastService
{
    Task<ForecastScenarioDto> GenerateBaseCaseForecastAsync(int daysAhead, CancellationToken ct = default);
    Task<ForecastScenarioDto> GenerateOptimisticForecastAsync(int daysAhead, CancellationToken ct = default);
    Task<ForecastScenarioDto> GeneratePessimisticForecastAsync(int daysAhead, CancellationToken ct = default);
    Task<IReadOnlyList<ForecastScenarioDto>> GetActiveForecastsAsync(CancellationToken ct = default);
}
