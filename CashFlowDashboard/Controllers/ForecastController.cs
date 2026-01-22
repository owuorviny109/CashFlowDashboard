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
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var forecasts = await _forecastService.GetActiveForecastsAsync(ct);
        var metrics = await _analyticsService.GetGrowthMetricsAsync(ct);

        // Select Base Case as primary scenario
        var activeScenario = forecasts.FirstOrDefault(f => f.ScenarioType == ScenarioType.BaseCase);

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
