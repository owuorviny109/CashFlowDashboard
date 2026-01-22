using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.ViewModels;

public sealed class ForecastViewModel
{
    // Primary scenario to display
    public ForecastScenarioDto? ActiveScenario { get; init; }
    
    // All available scenarios for comparison
    public IReadOnlyList<ForecastScenarioDto> AllScenarios { get; init; } = Array.Empty<ForecastScenarioDto>();
    
    // Derived metrics for KPI cards
    public decimal EndCashBalance { get; init; }
    public decimal BurnRatePerMonth { get; init; }
    public decimal GrowthRate { get; init; }
}
