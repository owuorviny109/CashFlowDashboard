using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.Services.Interfaces;

public interface ITransactionService
{
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionCommand cmd, CancellationToken ct = default);
    Task<TransactionDto?> GetTransactionByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TransactionDto>> GetRecentTransactionsAsync(int count, CancellationToken ct = default);
    Task<TransactionSummaryDto> GetSummaryForPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default);
    Task UpdateTransactionAsync(Guid id, UpdateTransactionCommand cmd, CancellationToken ct = default);
    Task DeleteTransactionAsync(Guid id, CancellationToken ct = default);
}
