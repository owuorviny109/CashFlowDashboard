using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.ViewModels;

public sealed class TransactionsViewModel
{
    public IReadOnlyList<TransactionDto> Transactions { get; init; } = Array.Empty<TransactionDto>();
    
    // Summary Metrics
    public TransactionSummaryDto Summary { get; init; } = new(0, 0, 0, 0, 0);

    // Filters
    public string? SearchTerm { get; init; }
    public string? TypeFilter { get; init; } // "Income", "Expense", or null
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }

    // Pagination
    public int TotalCount { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
