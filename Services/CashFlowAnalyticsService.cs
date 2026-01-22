using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Services.DTOs;
using CashFlowDashboard.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CashFlowDashboard.Services;

public class CashFlowAnalyticsService : ICashFlowAnalyticsService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICashFlowSnapshotRepository _snapshotRepository;
    private readonly ILogger<CashFlowAnalyticsService> _logger;

    public CashFlowAnalyticsService(
        ITransactionRepository transactionRepository,
        ICashFlowSnapshotRepository snapshotRepository,
        ILogger<CashFlowAnalyticsService> logger)
    {
        _transactionRepository = transactionRepository;
        _snapshotRepository = snapshotRepository;
        _logger = logger;
    }

    public async Task<decimal> GetCurrentBalanceAsync(CancellationToken ct = default)
    {
        return await _transactionRepository.GetBalanceAtDateAsync(DateTime.Today, ct);
    }

    public async Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct = default)
    {
        return await _transactionRepository.GetBalanceAtDateAsync(date, ct);
    }

    public async Task<CashFlowTrendDto> GetTrendForPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default)
    {
        // Optimization: Use pre-computed snapshots (materialized view pattern) for performance.
        // Queries raw transactions only if snapshots are missing (not implemented below for brevity, assumed snapshots exist).
        var snapshots = await _snapshotRepository.GetRangeAsync(DateOnly.FromDateTime(start), DateOnly.FromDateTime(end), ct);

        var points = snapshots.Select(s => new ChartDataPoint(
            Date: s.Date.ToString("yyyy-MM-dd"),
            Balance: s.ClosingBalance,
            Income: s.TotalIncome,
            Expenses: s.TotalExpenses
        )).ToList();

        // Calculate simple trend direction (Start vs End)
        TrendDirection direction = TrendDirection.Stable;
        decimal growthRate = 0;

        if (points.Count >= 2)
        {
            var first = points.First().Balance ?? 0;
            var last = points.Last().Balance ?? 0;
            
            if (last > first) direction = TrendDirection.Rising;
            else if (last < first) direction = TrendDirection.Falling;

            if (first != 0) 
            {
                growthRate = (last - first) / Math.Abs(first);
            }
        }

        return new CashFlowTrendDto(points, direction, growthRate);
    }

    public async Task<GrowthMetricsDto> GetGrowthMetricsAsync(CancellationToken ct = default)
    {
        var today = DateTime.Today;
        var currentBalance = await _transactionRepository.GetBalanceAtDateAsync(today, ct);
        var lastMonthBalance = await _transactionRepository.GetBalanceAtDateAsync(today.AddDays(-30), ct);

        decimal balanceChangePercent = 0;
        if (lastMonthBalance != 0)
        {
            balanceChangePercent = (currentBalance - lastMonthBalance) / Math.Abs(lastMonthBalance);
        }

        var recentTxns = await _transactionRepository.GetByDateRangeAsync(today.AddDays(-30), today, ct);
        
        var totalIncome = recentTxns.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpenses = recentTxns.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        var netFlow = totalIncome - totalExpenses;

        // Approximation: Burn Rate = Average monthly expenses over last 30 days
        var burnRate = totalExpenses; 

        return new GrowthMetricsDto(
            CurrentBalance: currentBalance,
            BalanceChangePercent: balanceChangePercent,
            NetCashFlow30Day: netFlow,
            BurnRate: burnRate
        );
    }

    public async Task GenerateDailySnapshotAsync(DateOnly date, CancellationToken ct = default)
    {
        // Idempotency check: Don't duplicate snapshots
        if (await _snapshotRepository.GetByDateAsync(date, ct) != null)
        {
            _logger.LogInformation("Snapshot for {Date} already exists. Skipping.", date);
            return;
        }

        var balance = await _transactionRepository.GetBalanceAtDateAsync(date.ToDateTime(TimeOnly.MinValue), ct);
        
        // Performance Note: 
        // Querying all transactions for the day is O(N) where N = txns in that day.
        // Acceptable for < 10k daily txns. Optimize with specialized DB query if scaling needed.
        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = date.ToDateTime(TimeOnly.MaxValue);
        var daysTransactions = await _transactionRepository.GetByDateRangeAsync(dayStart, dayEnd, ct);

        var totalIncome = daysTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpenses = daysTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var snapshot = new CashFlowSnapshot
        {
            Id = Guid.NewGuid(),
            Date = date,
            ClosingBalance = balance,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetCashFlow = totalIncome - totalExpenses,
            TransactionCount = daysTransactions.Count,
            OpeningBalance = balance - (totalIncome - totalExpenses), // Derived for consistency check
            ComputedAt = DateTime.UtcNow
        };

        await _snapshotRepository.AddAsync(snapshot, ct);
        _logger.LogInformation("Generated snapshot for {Date}", date);
    }
}
