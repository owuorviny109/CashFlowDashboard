using CashFlowDashboard.Models.Entities;

namespace CashFlowDashboard.Data.Repositories.Interfaces;

// Interface for Transaction data access.
// Defines contract for retrieving and modifying transaction records.
public interface ITransactionRepository
{
    // Retrieves a single transaction by its unique identifier.
    // Returns null if not found.
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Retrieves a list of transactions within a specified date range.
    // Results are ordered by date descending (newest first).
    Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken ct = default);

    // Retrieves the most recent N transactions.
    // Useful for dashboard widgets and recent activity feeds.
    Task<IReadOnlyList<Transaction>> GetRecentAsync(int count, CancellationToken ct = default);

    // Calculates the running balance (Income - Expenses) up to a specific date.
    // Includes transactions on the exact date provided.
    Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct = default);

    // Adds a new transaction to the database.
    Task AddAsync(Transaction transaction, CancellationToken ct = default);

    // Updates an existing transaction's details.
    Task UpdateAsync(Transaction transaction, CancellationToken ct = default);

    // Deletes a transaction by its unique identifier.
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    // Checks if a transaction exists with the given ID.
    // efficient method that avoids loading the entire entity.
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
