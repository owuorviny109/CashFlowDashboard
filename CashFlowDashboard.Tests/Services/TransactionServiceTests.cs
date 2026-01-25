using CashFlowDashboard.Data;
using CashFlowDashboard.Data.Repositories;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Services;
using CashFlowDashboard.Services.DTOs;
using CashFlowDashboard.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CashFlowDashboard.Tests.Services;

public class TransactionServiceTests
{
    private readonly Mock<ILogger<TransactionService>> _mockLogger;

    public TransactionServiceTests()
    {
        _mockLogger = new Mock<ILogger<TransactionService>>();
    }

    [Fact]
    public async Task CreateTransactionAsync_WithValidData_ReturnsTransactionDto()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CreateValid")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var command = new CreateTransactionCommand
        {
            Date = DateTime.UtcNow,
            Amount = 1000.50m,
            Type = TransactionType.Income,
            Category = "Sales",
            Description = "Test transaction",
            IsRecurring = false
        };

        // Act
        var result = await service.CreateTransactionAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(command.Amount, result.Amount);
        Assert.Equal(command.Type, result.Type);
        Assert.Equal(command.Category, result.Category);
        Assert.Equal(command.Description, result.Description);
        Assert.Equal(command.IsRecurring, result.IsRecurring);
        Assert.Contains("+", result.FormattedAmount);
    }

    [Fact]
    public async Task CreateTransactionAsync_WithZeroAmount_ThrowsValidationException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CreateZeroAmount")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var command = new CreateTransactionCommand
        {
            Date = DateTime.UtcNow,
            Amount = 0,
            Type = TransactionType.Income,
            Category = "Sales"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateTransactionAsync(command));
    }

    [Fact]
    public async Task CreateTransactionAsync_WithNegativeAmount_ThrowsValidationException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CreateNegativeAmount")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var command = new CreateTransactionCommand
        {
            Date = DateTime.UtcNow,
            Amount = -100,
            Type = TransactionType.Expense,
            Category = "Operating"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateTransactionAsync(command));
    }

    [Fact]
    public async Task CreateTransactionAsync_WithEmptyCategory_ThrowsValidationException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CreateEmptyCategory")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var command = new CreateTransactionCommand
        {
            Date = DateTime.UtcNow,
            Amount = 100,
            Type = TransactionType.Income,
            Category = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateTransactionAsync(command));
    }

    [Fact]
    public async Task CreateTransactionAsync_WithFutureDate_ThrowsValidationException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CreateFutureDate")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var command = new CreateTransactionCommand
        {
            Date = DateTime.UtcNow.AddYears(2),
            Amount = 100,
            Type = TransactionType.Income,
            Category = "Sales"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateTransactionAsync(command));
    }

    [Fact]
    public async Task GetTransactionByIdAsync_WithExistingId_ReturnsTransactionDto()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetById")
            .Options;

        using var context = new CashFlowDbContext(options);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Amount = 500,
            Type = TransactionType.Income,
            Category = "Sales",
            Description = "Test",
            CreatedAt = DateTime.UtcNow
        };
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act
        var result = await service.GetTransactionByIdAsync(transaction.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(transaction.Amount, result.Amount);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetByIdNotFound")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act
        var result = await service.GetTransactionByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRecentTransactionsAsync_ReturnsLimitedCount()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetRecent")
            .Options;

        using var context = new CashFlowDbContext(options);
        
        // Add 10 transactions
        for (int i = 0; i < 10; i++)
        {
            await context.Transactions.AddAsync(new Transaction
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow.AddDays(-i),
                Amount = 100 + i,
                Type = TransactionType.Income,
                Category = "Sales",
                CreatedAt = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act
        var result = await service.GetRecentTransactionsAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task GetFilteredTransactionsAsync_WithTypeFilter_ReturnsFilteredResults()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_FilterByType")
            .Options;

        using var context = new CashFlowDbContext(options);
        
        await context.Transactions.AddRangeAsync(new[]
        {
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 1000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 500, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 2000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act
        var (transactions, summary) = await service.GetFilteredTransactionsAsync(null, TransactionType.Income, null, null);

        // Assert
        Assert.NotNull(transactions);
        Assert.Equal(2, transactions.Count);
        Assert.All(transactions, t => Assert.Equal(TransactionType.Income, t.Type));
        Assert.Equal(3000, summary.TotalIncome);
        Assert.Equal(0, summary.TotalExpenses);
    }

    [Fact]
    public async Task GetFilteredTransactionsAsync_WithDateRange_ReturnsFilteredResults()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_FilterByDate")
            .Options;

        using var context = new CashFlowDbContext(options);
        var startDate = DateTime.UtcNow.AddDays(-10);
        var endDate = DateTime.UtcNow.AddDays(-5);
        
        await context.Transactions.AddRangeAsync(new[]
        {
            new Transaction { Id = Guid.NewGuid(), Date = startDate.AddDays(1), Amount = 1000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = endDate.AddDays(1), Amount = 500, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = startDate.AddDays(-1), Amount = 2000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act
        var (transactions, summary) = await service.GetFilteredTransactionsAsync(null, null, startDate, endDate);

        // Assert
        Assert.NotNull(transactions);
        Assert.Single(transactions);
        Assert.Equal(1000, transactions[0].Amount);
    }

    [Fact]
    public async Task GetFilteredTransactionsAsync_CalculatesSummaryCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_SummaryCalculation")
            .Options;

        using var context = new CashFlowDbContext(options);
        
        await context.Transactions.AddRangeAsync(new[]
        {
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 1000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 500, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 2000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 300, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act
        var (transactions, summary) = await service.GetFilteredTransactionsAsync(null, null, null, null);

        // Assert
        Assert.Equal(3000, summary.TotalIncome);
        Assert.Equal(800, summary.TotalExpenses);
        Assert.Equal(2200, summary.NetCashFlow);
        Assert.Equal(4, summary.TransactionCount);
        Assert.Equal(950, summary.AverageTransactionSize); // (3000 + 800) / 4
    }

    [Fact]
    public async Task GetSummaryForPeriodAsync_CalculatesCorrectSummary()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_SummaryPeriod")
            .Options;

        using var context = new CashFlowDbContext(options);
        var start = DateTime.UtcNow.AddDays(-5);
        var end = DateTime.UtcNow;
        
        await context.Transactions.AddRangeAsync(new[]
        {
            new Transaction { Id = Guid.NewGuid(), Date = start.AddDays(1), Amount = 1000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = start.AddDays(2), Amount = 500, Type = TransactionType.Expense, Category = "Operating", CreatedAt = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), Date = start.AddDays(3), Amount = 2000, Type = TransactionType.Income, Category = "Sales", CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act
        var summary = await service.GetSummaryForPeriodAsync(start, end);

        // Assert
        Assert.Equal(3000, summary.TotalIncome);
        Assert.Equal(500, summary.TotalExpenses);
        Assert.Equal(2500, summary.NetCashFlow);
        Assert.Equal(3, summary.TransactionCount);
    }

    [Fact]
    public async Task UpdateTransactionAsync_WithValidId_UpdatesTransaction()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Update")
            .Options;

        using var context = new CashFlowDbContext(options);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Amount = 1000,
            Type = TransactionType.Income,
            Category = "Sales",
            Description = "Original",
            CreatedAt = DateTime.UtcNow
        };
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var updateCommand = new UpdateTransactionCommand
        {
            Amount = 2000,
            Category = "Updated",
            Description = "Updated Description"
        };

        // Act
        await service.UpdateTransactionAsync(transaction.Id, updateCommand);

        // Assert
        await context.Entry(transaction).ReloadAsync();
        Assert.Equal(2000, transaction.Amount);
        Assert.Equal("Updated", transaction.Category);
        Assert.Equal("Updated Description", transaction.Description);
        Assert.NotNull(transaction.ModifiedAt);
    }

    [Fact]
    public async Task UpdateTransactionAsync_WithPartialUpdate_UpdatesOnlySpecifiedFields()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_PartialUpdate")
            .Options;

        using var context = new CashFlowDbContext(options);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Amount = 1000,
            Type = TransactionType.Income,
            Category = "Sales",
            Description = "Original",
            CreatedAt = DateTime.UtcNow
        };
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var updateCommand = new UpdateTransactionCommand
        {
            Amount = 2000
            // Category and Description not specified
        };

        // Act
        await service.UpdateTransactionAsync(transaction.Id, updateCommand);

        // Assert
        await context.Entry(transaction).ReloadAsync();
        Assert.Equal(2000, transaction.Amount);
        Assert.Equal("Sales", transaction.Category); // Unchanged
        Assert.Equal("Original", transaction.Description); // Unchanged
    }

    [Fact]
    public async Task UpdateTransactionAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_UpdateNotFound")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var updateCommand = new UpdateTransactionCommand { Amount = 2000 };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            service.UpdateTransactionAsync(Guid.NewGuid(), updateCommand));
    }

    [Fact]
    public async Task DeleteTransactionAsync_WithExistingId_DeletesTransaction()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Delete")
            .Options;

        using var context = new CashFlowDbContext(options);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Amount = 1000,
            Type = TransactionType.Income,
            Category = "Sales",
            CreatedAt = DateTime.UtcNow
        };
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act
        await service.DeleteTransactionAsync(transaction.Id);

        // Assert
        var deleted = await context.Transactions.FindAsync(transaction.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteTransactionAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_DeleteNotFound")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        // Act & Assert - Should not throw (idempotent)
        await service.DeleteTransactionAsync(Guid.NewGuid());
    }

    [Fact]
    public async Task CreateTransactionAsync_WithExpenseType_FormatsAmountCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CashFlowDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_ExpenseFormat")
            .Options;

        using var context = new CashFlowDbContext(options);
        var repository = new TransactionRepository(context);
        var service = new TransactionService(repository, _mockLogger.Object);

        var command = new CreateTransactionCommand
        {
            Date = DateTime.UtcNow,
            Amount = 500,
            Type = TransactionType.Expense,
            Category = "Operating"
        };

        // Act
        var result = await service.CreateTransactionAsync(command);

        // Assert
        Assert.Contains("-", result.FormattedAmount);
    }
}

