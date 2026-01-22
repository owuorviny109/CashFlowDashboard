using System.Diagnostics;
using CashFlowDashboard.Models;
using CashFlowDashboard.Services.Interfaces;
using CashFlowDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICashFlowAnalyticsService _analyticsService;
        private readonly IAlertService _alertService;
        private readonly IForecastService _forecastService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ICashFlowAnalyticsService analyticsService,
            IAlertService alertService,
            IForecastService forecastService,
            ITransactionService transactionService,
            ILogger<HomeController> logger)
        {
            _analyticsService = analyticsService;
            _alertService = alertService;
            _forecastService = forecastService;
            _transactionService = transactionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            try
            {
                // Aggregate data from multiple services (Controller orchestration pattern)
                var metricsTask = _analyticsService.GetGrowthMetricsAsync(ct);
                var trendTask = _analyticsService.GetTrendForPeriodAsync(
                    DateTime.Today.AddMonths(-6), 
                    DateTime.Today, 
                    ct);
                var alertsTask = _alertService.GetActiveAlertsAsync(ct);
                var forecastsTask = _forecastService.GetActiveForecastsAsync(ct);
                var recentTransactionsTask = _transactionService.GetRecentTransactionsAsync(10, ct);

                // Parallel execution for performance
                await Task.WhenAll(metricsTask, trendTask, alertsTask, forecastsTask, recentTransactionsTask);

                var metrics = await metricsTask;
                var trend = await trendTask;
                var alerts = await alertsTask;
                var forecasts = await forecastsTask;
                var recentTransactions = await recentTransactionsTask;

                // Calculate forecast growth (60-day projection end balance vs current)
                var forecast60Day = forecasts.FirstOrDefault(f => f.ScenarioType == Models.Enums.ScenarioType.BaseCase);
                decimal forecastGrowth = 0;
                if (forecast60Day != null && metrics.CurrentBalance != 0)
                {
                    forecastGrowth = (forecast60Day.EndCashBalance - metrics.CurrentBalance) / Math.Abs(metrics.CurrentBalance);
                }

                var model = new DashboardViewModel
                {
                    CurrentBalance = metrics.CurrentBalance,
                    BalanceChangePercent = metrics.BalanceChangePercent,
                    NetCashFlow30Day = metrics.NetCashFlow30Day,
                    ForecastGrowth60Day = forecastGrowth,
                    ActiveAlertCount = alerts.Count,
                    HistoricalChartData = trend.DataPoints,
                    ProjectedChartData = forecast60Day?.DataPoints.Select(dp => new Services.DTOs.ChartDataPoint(
                        Date: dp.Date.ToString("yyyy-MM-dd"),
                        Balance: dp.ProjectedBalance,
                        Income: null,
                        Expenses: null
                    )).ToList() ?? new List<Services.DTOs.ChartDataPoint>(),
                    RecentAlerts = alerts.Take(5).ToList(),
                    RecentTransactions = recentTransactions
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Graceful degradation: Show empty dashboard rather than crash
                _logger.LogError(ex, "Failed to load dashboard data");
                return View(new DashboardViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
