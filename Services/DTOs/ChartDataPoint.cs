namespace CashFlowDashboard.Services.DTOs;

public record ChartDataPoint(
    string Date, // ISO 8601 string for JS charts
    decimal? Balance,
    decimal? Income,
    decimal? Expenses
);
