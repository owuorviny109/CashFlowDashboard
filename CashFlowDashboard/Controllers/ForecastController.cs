using Microsoft.AspNetCore.Mvc;
using CashFlowDashboard.ViewModels;
using CashFlowDashboard.Services.Interfaces;
using CashFlowDashboard.Models.Enums;

namespace CashFlowDashboard.Controllers;

public class ForecastController : Controller
{
    private readonly IForecastService _forecastService;
    private readonly ICashFlowAnalyticsService _analyticsService;

    public ForecastController(IForecastService forecastService, ICashFlowAnalyticsService analyticsService)
    {
        _forecastService = forecastService;
        _analyticsService = analyticsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(ScenarioType? type, CancellationToken ct = default)
    {
        var forecasts = await _forecastService.GetActiveForecastsAsync(ct);
        var metrics = await _analyticsService.GetGrowthMetricsAsync(ct);

        // Select Scenario logic:
        // 1. If 'type' is provided, try to find that specific scenario.
        // 2. If not found or not provided, fallback to BaseCase.
        // 3. If BaseCase not found, fallback to the first available one.
        var activeScenario = forecasts.FirstOrDefault(f => f.ScenarioType == type) 
                             ?? forecasts.FirstOrDefault(f => f.ScenarioType == ScenarioType.BaseCase)
                             ?? forecasts.FirstOrDefault();

        var model = new ForecastViewModel
        {
            ActiveScenario = activeScenario,
            AllScenarios = forecasts,
            EndCashBalance = activeScenario?.EndCashBalance ?? 0,
            BurnRatePerMonth = metrics.BurnRate,
            GrowthRate = metrics.BalanceChangePercent
        };

        return View(model);
    }
    
    [HttpPost]
    public IActionResult SwitchScenario(ScenarioType type)
    {
        // Simply redirect back to Index with the selected type query param
        return RedirectToAction(nameof(Index), new { type = type });
    }

    [HttpPost]
    public async Task<IActionResult> GenerateForecast(ScenarioType type, int daysAhead = 30, CancellationToken ct = default)
    {
        // Generate new forecast based on scenario type
        _ = type switch
        {
            ScenarioType.BaseCase => await _forecastService.GenerateBaseCaseForecastAsync(daysAhead, ct),
            ScenarioType.Optimistic => await _forecastService.GenerateOptimisticForecastAsync(daysAhead, ct),
            ScenarioType.Pessimistic => await _forecastService.GeneratePessimisticForecastAsync(daysAhead, ct),
            _ => throw new ArgumentException("Invalid scenario type", nameof(type))
        };

        return RedirectToAction(nameof(Index));
    }
}
