using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CashFlowDashboard.Data.Repositories;

// Implementation of ITransactionRepository using EF Core.
public class TransactionRepository : ITransactionRepository
{
    private readonly CashFlowDbContext _context;

    public TransactionRepository(CashFlowDbContext context)
    {
        _context = context;
    }

    // Retrieves a single transaction by its unique identifier.
    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Transactions.FindAsync(new object[] { id }, ct);
    }

    // Retrieves a list of transactions within a specified date range.
    // Uses AsNoTracking for read optimization.
    public async Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken ct = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.Date >= start && t.Date <= end)
            .OrderByDescending(t => t.Date)
            .ToListAsync(ct);
    }

    // Retrieves the most recent N transactions.
    public async Task<IReadOnlyList<Transaction>> GetRecentAsync(int count, CancellationToken ct = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .OrderByDescending(t => t.Date)
            .Take(count)
            .ToListAsync(ct);
    }

    // Calculates the running balance (Income - Expenses) up to a specific date.
    // Performs aggregation in the database for efficiency.
    public async Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct = default)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.Date <= date);

        // Fix for SQLite "Cannot apply aggregate operator Sum on decimal" error
        // We project only what we need (Type, Amount) and sum in memory.
        var transactions = await query
            .Select(t => new { t.Type, t.Amount })
            .ToListAsync(ct);

        var income = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var expense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        return income - expense;
    }

    // Adds a new transaction to the database.
    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await _context.Transactions.AddAsync(transaction, ct);
        await _context.SaveChangesAsync(ct);
    }

    // Updates an existing transaction's details.
    public async Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync(ct);
    }

    // Deletes a transaction by its unique identifier.
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var transaction = await _context.Transactions.FindAsync(new object[] { id }, ct);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync(ct);
        }
    }

    // Checks if a transaction exists with the given ID.
    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Transactions.AnyAsync(t => t.Id == id, ct);
    }
}
