using CashFlowDashboard.Data;
using CashFlowDashboard.Data.Repositories;
using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Services;
using CashFlowDashboard.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CashFlowDashboard.Tests.Services;

public class ForecastServiceTests
{
    private readonly Mock<ILogger<ForecastService>> _mockLogger;
    private readonly ForecastSettings _settings;

    public ForecastServiceTests()
    {
        _mockLogger = new Mock<ILogger<ForecastService>>();
        _settings = new ForecastSettings
        {
            HistoryDaysToAnalyze = 90,
            DefaultHorizonDays = 30
        };
    }

    [Fact]
    public async Task GenerateBaseCaseForecastAsync_WithValidData_ReturnsScenario()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_BaseCase")
            .Options;

        using var context = new CashFlowDbContext(options);
        await SeedTestDataAsync(context);

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var forecastRepo = new ForecastRepository(context);

        var service = new ForecastService(
            transactionRepo,
            snapshotRepo,
            forecastRepo,
            Options.Create(_settings),
            _mockLogger.Object
        );

        // Act
        var result = await service.GenerateBaseCaseForecastAsync(30);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ScenarioType.BaseCase, result.ScenarioType);
        Assert.NotEmpty(result.DataPoints);
        Assert.True(result.DataPoints.Count <= 30);
    }

    [Fact]
    public async Task GenerateOptimisticForecastAsync_ReturnsHigherProjections()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Optimistic")
            .Options;

        using var context = new CashFlowDbContext(options);
        await SeedTestDataAsync(context);

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var forecastRepo = new ForecastRepository(context);

        var service = new ForecastService(
            transactionRepo,
            snapshotRepo,
            forecastRepo,
            Options.Create(_settings),
            _mockLogger.Object
        );

        // Act
        var baseCase = await service.GenerateBaseCaseForecastAsync(30);
        var optimistic = await service.GenerateOptimisticForecastAsync(30);

        // Assert
        Assert.NotNull(optimistic);
        Assert.Equal(ScenarioType.Optimistic, optimistic.ScenarioType);
        
        // Optimistic should generally have higher balances (if trend is positive)
        var baseFinalBalance = baseCase.DataPoints.Last().ProjectedBalance;
        var optFinalBalance = optimistic.DataPoints.Last().ProjectedBalance;
        
        // This assertion depends on having positive cash flow trend
        Assert.True(optFinalBalance >= baseFinalBalance || baseFinalBalance < 0);
    }

    [Fact]
    public async Task GeneratePessimisticForecastAsync_ReturnsLowerProjections()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Pessimistic")
            .Options;

        using var context = new CashFlowDbContext(options);
        await SeedTestDataAsync(context);

        var transactionRepo = new TransactionRepository(context);
        var snapshotRepo = new CashFlowSnapshotRepository(context);
        var forecastRepo = new ForecastRepository(context);

        var service = new ForecastService(
            transactionRepo,
            snapshotRepo,
            forecastRepo,
            Options.Create(_settings),
            _mockLogger.Object
        );

        // Act
        var baseCase = await service.GenerateBaseCaseForecastAsync(30);
        var pessimistic = await service.GeneratePessimisticForecastAsync(30);

        // Assert
        Assert.NotNull(pessimistic);
        Assert.Equal(ScenarioType.Pessimistic, pessimistic.ScenarioType);
        
        var baseFinalBalance = baseCase.DataPoints.Last().ProjectedBalance;
        var pessFinalBalance = pessimistic.DataPoints.Last().ProjectedBalance;
        
        // Pessimistic should have lower balances
        Assert.True(pessFinalBalance <= baseFinalBalance || baseFinalBalance < 0);
    }

    private async Task SeedTestDataAsync(CashFlowDbContext context)
    {
        // Seed 60 days of transactions for testing
        var transactions = new List<Transaction>();
        var snapshots = new List<CashFlowSnapshot>();
        
        var startDate = DateTime.Today.AddDays(-60);
        decimal balance = 50000;

        for (int i = 0; i < 60; i++)
        {
            var date = startDate.AddDays(i);
            
            // Add some income
            transactions.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                Date = date.AddHours(10),
                Amount = 5000,
                Type = TransactionType.Income,
                Category = "Sales",
                Description = "Test Income",
                CreatedAt = DateTime.UtcNow
            });

            // Add some expenses
            transactions.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                Date = date.AddHours(14),
                Amount = 3000,
                Type = TransactionType.Expense,
                Category = "Operating",
                Description = "Test Expense",
                CreatedAt = DateTime.UtcNow
            });

            // Create snapshot
            decimal dayIncome = 5000;
            decimal dayExpense = 3000;
            decimal netFlow = dayIncome - dayExpense;
            balance += netFlow;

            snapshots.Add(new CashFlowSnapshot
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(date),
                OpeningBalance = balance - netFlow,
                ClosingBalance = balance,
                TotalIncome = dayIncome,
                TotalExpenses = dayExpense,
                NetCashFlow = netFlow,
                TransactionCount = 2,
                ComputedAt = DateTime.UtcNow
            });
        }

        await context.Transactions.AddRangeAsync(transactions);
        await context.Snapshots.AddRangeAsync(snapshots);
        await context.SaveChangesAsync();
    }
}
