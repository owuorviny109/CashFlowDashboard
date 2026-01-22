using CashFlowDashboard.Models.Enums;

namespace CashFlowDashboard.Services.DTOs;

public record AlertDto(
    Guid Id,
    AlertSeverity Severity,
    string Title,
    string Message,
    string TimeAgo,
    AlertStatus Status,
    string? ActionUrl,
    DateTime GeneratedAt
);
