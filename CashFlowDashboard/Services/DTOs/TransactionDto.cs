using CashFlowDashboard.Models.Enums;

namespace CashFlowDashboard.Services.DTOs;

public record TransactionDto(
    Guid Id,
    DateTime Date,
    string Description,
    string Category,
    TransactionType Type,
    decimal Amount,
    string FormattedAmount,
    bool IsRecurring,
    RecurrencePattern? RecurrencePattern
);
