using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.Services.Interfaces;

public interface ITransactionService
{
    /// <summary>
    /// Creates a new transaction from the provided command.
    /// </summary>
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionCommand cmd, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a specific transaction by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    Task<TransactionDto?> GetTransactionByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a list of the most recent transactions, limited by the count.
    /// </summary>
    Task<IReadOnlyList<TransactionDto>> GetRecentTransactionsAsync(int count, CancellationToken ct = default);

    /// <summary>
    /// Calculates income, expenses, and net balance for a specified date range.
    /// </summary>
    Task<TransactionSummaryDto> GetSummaryForPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing transaction. Throws NotFoundException if the ID does not exist.
    /// </summary>
    Task UpdateTransactionAsync(Guid id, UpdateTransactionCommand cmd, CancellationToken ct = default);

    /// <summary>
    /// Deletes a transaction by its unique identifier.
    /// </summary>
    Task DeleteTransactionAsync(Guid id, CancellationToken ct = default);
}
