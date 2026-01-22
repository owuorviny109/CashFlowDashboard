namespace CashFlowDashboard.Services.DTOs;

public record TransactionSummaryDto(
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetCashFlow,
    int TransactionCount,
    decimal AverageTransactionSize
);
