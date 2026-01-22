using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.ViewModels;

public sealed class TransactionsViewModel
{
    public IReadOnlyList<TransactionDto> Transactions { get; init; } = Array.Empty<TransactionDto>();
    
    // Pagination
    public int TotalCount { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
