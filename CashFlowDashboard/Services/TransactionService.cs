using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Services.DTOs;
using CashFlowDashboard.Services.Exceptions;
using CashFlowDashboard.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CashFlowDashboard.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository repository, ILogger<TransactionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionCommand cmd, CancellationToken ct = default)
    {
        // Validation: Amount must be positive
        if (cmd.Amount <= 0)
        {
            throw new ValidationException("Transaction amount must be greater than zero.");
        }

        // Validation: Category is required
        if (string.IsNullOrWhiteSpace(cmd.Category))
        {
            throw new ValidationException("Transaction category is required.");
        }

        // Validation: Date cannot be in the far future (sanity check)
        if (cmd.Date > DateTime.UtcNow.AddYears(1))
        {
            throw new ValidationException("Transaction date cannot be more than 1 year in the future.");
        }

        _logger.LogInformation("Creating transaction: {Type} {Amount:C} - {Category}", cmd.Type, cmd.Amount, cmd.Category);

        try
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Date = cmd.Date,
                Amount = cmd.Amount,
                Type = cmd.Type,
                Category = cmd.Category,
                Description = cmd.Description,
                IsRecurring = cmd.IsRecurring,
                RecurrencePattern = cmd.RecurrencePattern,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(transaction, ct);

            _logger.LogInformation("Transaction created successfully with ID {TransactionId}", transaction.Id);
            return ToDto(transaction);
        }
        catch (Exception ex) when (ex is not ValidationException)
        {
            _logger.LogError(ex, "Failed to create transaction: {Type} {Amount:C}", cmd.Type, cmd.Amount);
            throw;
        }
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(Guid id, CancellationToken ct = default)
    {
        var transaction = await _repository.GetByIdAsync(id, ct);
        return transaction == null ? null : ToDto(transaction);
    }

    public async Task<IReadOnlyList<TransactionDto>> GetRecentTransactionsAsync(int count, CancellationToken ct = default)
    {
        var transactions = await _repository.GetRecentAsync(count, ct);
        return transactions.Select(ToDto).ToList();
    }

    public async Task<TransactionSummaryDto> GetSummaryForPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default)
    {
        var transactions = await _repository.GetByDateRangeAsync(start, end, ct);

        // Assumption: Data set is small enough to aggregate in-memory.
        // TODO: Move aggregation to DB query (Group By) if volume exceeds 10k records per period.
        decimal totalIncome = 0;
        decimal totalExpenses = 0;
        int count = 0;

        foreach (var txn in transactions)
        {
            if (txn.Type == TransactionType.Income)
                totalIncome += txn.Amount;
            else
                totalExpenses += txn.Amount;

            count++;
        }

        var netCashFlow = totalIncome - totalExpenses;
        var avgSize = count > 0 ? (totalIncome + totalExpenses) / count : 0;

        return new TransactionSummaryDto(
            TotalIncome: totalIncome,
            TotalExpenses: totalExpenses,
            NetCashFlow: netCashFlow,
            TransactionCount: count,
            AverageTransactionSize: avgSize
        );
    }

    public async Task UpdateTransactionAsync(Guid id, UpdateTransactionCommand cmd, CancellationToken ct = default)
    {
        var transaction = await _repository.GetByIdAsync(id, ct);
        if (transaction == null)
        {
            _logger.LogWarning("Attempted to update non-existent transaction {Id}", id);
            throw new NotFoundException("Transaction", id);
        }

        bool modified = false;

        // Patch-style update: only field values present in command are applied
        if (cmd.Amount.HasValue && cmd.Amount.Value != transaction.Amount)
        {
            transaction.Amount = cmd.Amount.Value;
            modified = true;
        }

        if (cmd.Category != null && cmd.Category != transaction.Category)
        {
            transaction.Category = cmd.Category;
            modified = true;
        }

        if (cmd.Description != null && cmd.Description != transaction.Description)
        {
            transaction.Description = cmd.Description;
            modified = true;
        }

        if (modified)
        {
            transaction.ModifiedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(transaction, ct);
            _logger.LogInformation("Updated transaction {Id}", id);
        }
    }

    public async Task DeleteTransactionAsync(Guid id, CancellationToken ct = default)
    {
        // Idempotent: If it doesn't exist, we consider it effectively deleted.
        if (!await _repository.ExistsAsync(id, ct))
        {
             _logger.LogWarning("Attempted to delete non-existent transaction {Id}", id);
             return; 
        }

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("Deleted transaction {Id}", id);
    }

    private static TransactionDto ToDto(Transaction txn)
    {
        // Format amount: Income is positive, Expense is negative for display if desired, 
        // but typically raw amount + type is cleaner. 
        // Let's just format the string representation for UI convenience.
        string sign = txn.Type == TransactionType.Income ? "+" : "-";
        string formatted = $"{sign}{txn.Amount:C0}";

        return new TransactionDto(
            Id: txn.Id,
            Date: txn.Date,
            Description: txn.Description,
            Category: txn.Category,
            Type: txn.Type,
            Amount: txn.Amount,
            FormattedAmount: formatted,
            IsRecurring: txn.IsRecurring,
            RecurrencePattern: txn.RecurrencePattern
        );
    }
}
