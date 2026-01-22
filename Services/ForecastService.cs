using CashFlowDashboard.Configuration;
using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Models.ValueObjects;
using CashFlowDashboard.Services.DTOs;
using CashFlowDashboard.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CashFlowDashboard.Services;

public class ForecastService : IForecastService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICashFlowSnapshotRepository _snapshotRepository;
    private readonly IForecastRepository _forecastRepository;
    private readonly ForecastSettings _settings;
    private readonly ILogger<ForecastService> _logger;

    public ForecastService(
        ITransactionRepository transactionRepository,
        ICashFlowSnapshotRepository snapshotRepository,
        IForecastRepository forecastRepository,
        IOptions<ForecastSettings> settings,
        ILogger<ForecastService> logger)
    {
        _transactionRepository = transactionRepository;
        _snapshotRepository = snapshotRepository;
        _forecastRepository = forecastRepository;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ForecastScenarioDto> GenerateBaseCaseForecastAsync(int daysAhead, CancellationToken ct = default)
    {
        return await GenerateForecastAsync(ScenarioType.BaseCase, daysAhead, growthMultiplier: 1.0m, ct);
    }

    public async Task<ForecastScenarioDto> GenerateOptimisticForecastAsync(int daysAhead, CancellationToken ct = default)
    {
        // Optimistic: 20% higher daily trend, or just generally more aggressive growth
        return await GenerateForecastAsync(ScenarioType.Optimistic, daysAhead, growthMultiplier: 1.2m, ct);
    }

    public async Task<ForecastScenarioDto> GeneratePessimisticForecastAsync(int daysAhead, CancellationToken ct = default)
    {
        // Pessimistic: 20% lower daily trend (or negative growth amplified)
        return await GenerateForecastAsync(ScenarioType.Pessimistic, daysAhead, growthMultiplier: 0.8m, ct);
    }

    public async Task<IReadOnlyList<ForecastScenarioDto>> GetActiveForecastsAsync(CancellationToken ct = default)
    {
        var scenarios = await _forecastRepository.GetActiveScenariosAsync(ct);
        return scenarios.Select(ToDto).ToList();
    }

    private async Task<ForecastScenarioDto> GenerateForecastAsync(
        ScenarioType type, 
        int daysAhead, 
        decimal growthMultiplier,
        CancellationToken ct)
    {
        // 1. Data Collection
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        var startDate = endDate.AddDays(-_settings.HistoryDaysToAnalyze);
        
        var snapshots = await _snapshotRepository.GetRangeAsync(startDate, endDate, ct);
        
        // 2. Trend Analysis
        decimal dailyTrend = 0;
        decimal stdDev = 0;
        decimal currentBalance = await _transactionRepository.GetBalanceAtDateAsync(DateTime.Today, ct);

        if (snapshots.Count < 30)
        {
            // Fallback: Simple Average.
            // Why: Linear regression is unstable with fewer than 30 data points.
            if (snapshots.Count > 0)
            {
                var totalNetFlow = snapshots.Sum(s => s.NetCashFlow);
                dailyTrend = totalNetFlow / snapshots.Count;
                
                // Calculate simple StdDev for baseline variance
                double avg = (double)dailyTrend;
                double sumSqDiff = snapshots.Sum(s => Math.Pow((double)s.NetCashFlow - avg, 2));
                stdDev = (decimal)Math.Sqrt(sumSqDiff / snapshots.Count);
            }
        }
        else
        {
            // Linear Regression on Daily Net Cash Flow
            // Goal: Predict future "Daily Change" (Slope) rather than absolute Balance.
            // X = Day Index, Y = Net Cash Flow
            
            // Note: Regressing on 'Net Flow' catches the momentum (improving vs worsening cash flow).
            // Regressing on 'Balance' directly is often too noisy due to lump sum payments.
            
            int n = snapshots.Count;
            decimal sumX = 0;
            decimal sumY = 0;
            decimal sumXY = 0;
            decimal sumXX = 0;

            for (int i = 0; i < n; i++)
            {
                decimal x = i;
                decimal y = snapshots[i].NetCashFlow;
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumXX += x * x;
            }

            decimal slope = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
            decimal intercept = (sumY - slope * sumX) / n;
            
            // StdDev for confidence intervals
            double avgY = (double)(sumY / n);
            double sumSqDiff = snapshots.Sum(s => Math.Pow((double)s.NetCashFlow - avgY, 2));
            stdDev = (decimal)Math.Sqrt(sumSqDiff / n);

            // Store slope for projection loop
            dailyTrend = slope; 
        }

        // 3. Projection
        var dataPoints = new List<ForecastDataPoint>();
        decimal runningBalance = currentBalance;
        
        // Strategy: 
        // If > 30 days: Use Regression (Flow varies by day index).
        // If < 30 days: Use Simple Average (Flow is constant).
        decimal slopeVal = snapshots.Count >= 30 ? dailyTrend : 0;
        decimal interceptVal = snapshots.Count >= 30 
            ? (snapshots.Sum(s => s.NetCashFlow) - slopeVal * (snapshots.Count * (snapshots.Count - 1) / 2m)) / snapshots.Count 
            : dailyTrend; 

        for (int i = 1; i <= daysAhead; i++)
        {
            // Project Daily Flow: (Slope * FutureIndex) + Intercept
            // We continue the time series from where history left off (Count + i).
            decimal projectedDailyFlow = (slopeVal * (snapshots.Count + i)) + interceptVal;
            
            // Apply Growth Multiplier
            // Why: Scenarios (Optimistic/Pessimistic) assume structural changes in flow magnitude.
            projectedDailyFlow *= growthMultiplier;

            runningBalance += projectedDailyFlow;

            // Confidence Bounds (2 Sigma @ 95%)
            // Theory: Random Walk. Uncertainty grows with square root of time (Sqrt(t)).
            decimal width = 2 * stdDev * (decimal)Math.Sqrt(i);

            dataPoints.Add(new ForecastDataPoint(
                Date: DateOnly.FromDateTime(DateTime.Today.AddDays(i)),
                ProjectedBalance: runningBalance,
                LowerBound: runningBalance - width,
                UpperBound: runningBalance + width,
                Confidence: 0.95m 
            ));
        }

        // 4. Build Scenario
        var scenario = new ForecastScenario
        {
            Id = Guid.NewGuid(),
            Name = $"{type} Forecast ({daysAhead} days)",
            ScenarioType = type,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(daysAhead)),
            DataPoints = dataPoints,
            ConfidenceLevel = snapshots.Count >= 30 ? 0.85m : 0.50m, // Confidence drops with less data
            GeneratedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _forecastRepository.AddAsync(scenario, ct);

        return ToDto(scenario);
    }

    private static ForecastScenarioDto ToDto(ForecastScenario scenario)
    {
        return new ForecastScenarioDto(
            Id: scenario.Id,
            Name: scenario.Name,
            ScenarioType: scenario.ScenarioType,
            DataPoints: scenario.DataPoints.Select(dp => new ForecastDataPointDto(
                Date: dp.Date,
                ProjectedBalance: dp.ProjectedBalance,
                LowerBound: dp.LowerBound,
                UpperBound: dp.UpperBound
            )).ToList(),
            ConfidenceLevel: scenario.ConfidenceLevel,
            EndCashBalance: scenario.DataPoints.LastOrDefault()?.ProjectedBalance ?? 0
        );
    }
}
