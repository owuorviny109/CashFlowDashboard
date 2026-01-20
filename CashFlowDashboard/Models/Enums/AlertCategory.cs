namespace CashFlowDashboard.Models.Enums;

// Categorizes alerts by their functional domain for filtering and organization.
public enum AlertCategory
{
    // Alerts related to cash flow analysis and balance thresholds.
    CashFlow = 0,

    // Alerts related to invoices, payments, and receivables.
    Invoice = 1,

    // Alerts generated from forecast projections and predictions.
    Forecast = 2,

    // Security-related alerts (e.g., suspicious transactions).
    Security = 3,

    // System-level alerts (e.g., scheduled maintenance, data sync issues).
    System = 4
}
