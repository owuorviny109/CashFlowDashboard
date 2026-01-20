namespace CashFlowDashboard.Models.Enums;

// Defines the frequency pattern for recurring transactions.
public enum RecurrencePattern
{
    // One-time transaction, not recurring.
    None = 0,

    // Recurs every day.
    Daily = 1,

    // Recurs every week.
    Weekly = 2,

    // Recurs every month.
    Monthly = 3,

    // Recurs every quarter (3 months).
    Quarterly = 4,

    // Recurs every year.
    Annually = 5
}
