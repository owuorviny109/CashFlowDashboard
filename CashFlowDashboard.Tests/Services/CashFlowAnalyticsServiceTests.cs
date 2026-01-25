using CashFlowDashboard.Data;
using CashFlowDashboard.Data.Repositories;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CashFlowDashboard.Tests.Services;

public class CashFlowAnalyticsServiceTests
{
    private readonly Mock<ILogger<CashFlowAnalyticsService>> _mockLogger;

    public CashFlowAnalyticsServiceTests()
    {
        _mockLogger = new Mock<ILogger<CashFlowAnalyticsService>>();
    }

    [Fact]
    public async Task GetCurrentBalanceAsync_ReturnsBalanceForToday()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CurrentBalance")
            .Options;

        using var context = new CashFlowDbContext(options);
        
        // Create transactions that result in a balance
        await context.Transactions.AddRangeAsync(new[]
        {
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.Today, Amount = 10000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.Today, Amount = 3000, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        var result = await service.GetCurrentBalanceAsync();

        // Assert
        Assert.True(result > 0); // Should have positive balance
    }

    [Fact]
    public async Task GetBalanceAtDateAsync_ReturnsBalanceForSpecificDate()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_BalanceAtDate")
            .Options;

        using var context = new CashFlowDbContext(options);
        var targetDate = DateTime.Today.AddDays(-5);
        
        await context.Transactions.AddRangeAsync(new[]
        {
            new Transaction { Id = Guid.NewGuid(), Date = targetDate, Amount = 5000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = targetDate, Amount = 2000, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        var result = await service.GetBalanceAtDateAsync(targetDate);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public async Task GetTrendForPeriodAsync_WithRisingTrend_ReturnsRisingDirection()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_RisingTrend")
            .Options;

        using var context = new CashFlowDbContext(options);
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10));
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        
        // Create snapshots with rising balance
        await context.Snapshots.AddRangeAsync(new[]
        {
            new CashFlowSnapshot { Id = Guid.NewGuid(), Date = startDate, OpeningBalance = 10000, ClosingBalance = 11000, TotalIncome = 2000, TotalExpenses = 1000, NetCashFlow = 1000, TransactionCount = 2, ComputedAt = DateTime.UtcNow },
            new CashFlowSnapshot { Id = Guid.NewGuid(), Date = startDate.AddDays(5), OpeningBalance = 11000, ClosingBalance = 13000, TotalIncome = 3000, TotalExpenses = 1000, NetCashFlow = 2000, TransactionCount = 2, ComputedAt = DateTime.UtcNow },
            new CashFlowSnapshot { Id = Guid.NewGuid(), Date = endDate, OpeningBalance = 13000, ClosingBalance = 15000, TotalIncome = 3000, TotalExpenses = 1000, NetCashFlow = 2000, TransactionCount = 2, ComputedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        var result = await service.GetTrendForPeriodAsync(startDate.ToDateTime(TimeOnly.MinValue), endDate.ToDateTime(TimeOnly.MinValue));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TrendDirection.Rising, result.Direction);
        Assert.True(result.GrowthRate > 0);
        Assert.Equal(3, result.DataPoints.Count);
    }

    [Fact]
    public async Task GetTrendForPeriodAsync_WithFallingTrend_ReturnsFallingDirection()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_FallingTrend")
            .Options;

        using var context = new CashFlowDbContext(options);
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10));
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        
        // Create snapshots with falling balance
        await context.Snapshots.AddRangeAsync(new[]
        {
            new CashFlowSnapshot { Id = Guid.NewGuid(), Date = startDate, OpeningBalance = 15000, ClosingBalance = 13000, TotalIncome = 1000, TotalExpenses = 3000, NetCashFlow = -2000, TransactionCount = 2, ComputedAt = DateTime.UtcNow },
            new CashFlowSnapshot { Id = Guid.NewGuid(), Date = startDate.AddDays(5), OpeningBalance = 13000, ClosingBalance = 11000, TotalIncome = 1000, TotalExpenses = 3000, NetCashFlow = -2000, TransactionCount = 2, ComputedAt = DateTime.UtcNow },
            new CashFlowSnapshot { Id = Guid.NewGuid(), Date = endDate, OpeningBalance = 11000, ClosingBalance = 10000, TotalIncome = 1000, TotalExpenses = 2000, NetCashFlow = -1000, TransactionCount = 2, ComputedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        var result = await service.GetTrendForPeriodAsync(startDate.ToDateTime(TimeOnly.MinValue), endDate.ToDateTime(TimeOnly.MinValue));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TrendDirection.Falling, result.Direction);
        Assert.True(result.GrowthRate < 0);
    }

    [Fact]
    public async Task GetTrendForPeriodAsync_WithStableTrend_ReturnsStableDirection()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_StableTrend")
            .Options;

        using var context = new CashFlowDbContext(options);
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10));
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        
        // Create snapshots with stable balance
        await context.Snapshots.AddRangeAsync(new[]
        {
            new CashFlowSnapshot { Id = Guid.NewGuid(), Date = startDate, OpeningBalance = 10000, ClosingBalance = 10000, TotalIncome = 1000, TotalExpenses = 1000, NetCashFlow = 0, TransactionCount = 2, ComputedAt = DateTime.UtcNow },
            new CashFlowSnapshot { Id = Guid.NewGuid(), Date = endDate, OpeningBalance = 10000, ClosingBalance = 10000, TotalIncome = 1000, TotalExpenses = 1000, NetCashFlow = 0, TransactionCount = 2, ComputedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        var result = await service.GetTrendForPeriodAsync(startDate.ToDateTime(TimeOnly.MinValue), endDate.ToDateTime(TimeOnly.MinValue));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TrendDirection.Stable, result.Direction);
        Assert.Equal(0, result.GrowthRate);
    }

    [Fact]
    public async Task GetGrowthMetricsAsync_CalculatesMetricsCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GrowthMetrics")
            .Options;

        using var context = new CashFlowDbContext(options);
        var today = DateTime.Today;
        var thirtyDaysAgo = today.AddDays(-30);
        
        // Create transactions for current period
        await context.Transactions.AddRangeAsync(new[]
        {
            new Transaction { Id = Guid.NewGuid(), Date = today, Amount = 10000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = today, Amount = 5000, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = thirtyDaysAgo, Amount = 5000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        var result = await service.GetGrowthMetricsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CurrentBalance >= 0);
        Assert.True(result.NetCashFlow30Day >= 0);
        Assert.True(result.BurnRate >= 0);
    }

    [Fact]
    public async Task GenerateDailySnapshotAsync_WithValidData_CreatesSnapshot()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GenerateSnapshot")
            .Options;

        using var context = new CashFlowDbContext(options);
        var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var dayStart = targetDate.ToDateTime(TimeOnly.MinValue);
        var dayEnd = targetDate.ToDateTime(TimeOnly.MaxValue);
        
        // Create transactions for the target date
        await context.Transactions.AddRangeAsync(new[]
        {
            new Transaction { Id = Guid.NewGuid(), Date = dayStart.AddHours(10), Amount = 5000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = dayStart.AddHours(14), Amount = 2000, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        await service.GenerateDailySnapshotAsync(targetDate);

        // Assert
        var snapshot = await context.Snapshots.FirstOrDefaultAsync(s => s.Date == targetDate);
        Assert.NotNull(snapshot);
        Assert.Equal(5000, snapshot.TotalIncome);
        Assert.Equal(2000, snapshot.TotalExpenses);
        Assert.Equal(3000, snapshot.NetCashFlow);
        Assert.Equal(2, snapshot.TransactionCount);
    }

    [Fact]
    public async Task GenerateDailySnapshotAsync_WhenSnapshotExists_DoesNotCreateDuplicate()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_SnapshotIdempotent")
            .Options;

        using var context = new CashFlowDbContext(options);
        var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        
        // Create existing snapshot
        var existingSnapshot = new CashFlowSnapshot
        {
            Id = Guid.NewGuid(),
            Date = targetDate,
            OpeningBalance = 10000,
            ClosingBalance = 13000,
            TotalIncome = 5000,
            TotalExpenses = 2000,
            NetCashFlow = 3000,
            TransactionCount = 2,
            ComputedAt = DateTime.UtcNow.AddHours(-1)
        };
        await context.Snapshots.AddAsync(existingSnapshot);
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        await service.GenerateDailySnapshotAsync(targetDate);

        // Assert - Should still have only one snapshot
        var snapshotCount = await context.Snapshots.CountAsync(s => s.Date == targetDate);
        Assert.Equal(1, snapshotCount);
        
        // Verify the original snapshot wasn't modified
        await context.Entry(existingSnapshot).ReloadAsync();
        Assert.Equal(DateTime.UtcNow.AddHours(-1).Date, existingSnapshot.ComputedAt.Date);
    }

    [Fact]
    public async Task GetTrendForPeriodAsync_WithEmptySnapshots_ReturnsStableTrend()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_EmptyTrend")
            .Options;

        using var context = new CashFlowDbContext(options);
        var startDate = DateTime.Today.AddDays(-10);
        var endDate = DateTime.Today;

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        var result = await service.GetTrendForPeriodAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TrendDirection.Stable, result.Direction);
        Assert.Equal(0, result.GrowthRate);
        Assert.Empty(result.DataPoints);
    }

    [Fact]
    public async Task GetTrendForPeriodAsync_WithSingleSnapshot_ReturnsStableTrend()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_SingleSnapshotTrend")
            .Options;

        using var context = new CashFlowDbContext(options);
        var date = DateOnly.FromDateTime(DateTime.Today);
        
        await context.Snapshots.AddAsync(new CashFlowSnapshot
        {
            Id = Guid.NewGuid(),
            Date = date,
            OpeningBalance = 10000,
            ClosingBalance = 10000,
            TotalIncome = 1000,
            TotalExpenses = 1000,
            NetCashFlow = 0,
            TransactionCount = 2,
            ComputedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var service = new CashFlowAnalyticsService(transactionRepo, snapshotRepo, _mockLogger.Object);

        // Act
        var result = await service.GetTrendForPeriodAsync(date.ToDateTime(TimeOnly.MinValue), date.ToDateTime(TimeOnly.MinValue));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TrendDirection.Stable, result.Direction);
        Assert.Single(result.DataPoints);
    }
}

