namespace CashFlowDashboard.Configuration;

public class AlertSettings
{
    public decimal LowBalanceThreshold { get; set; } = 10000m;
    public decimal LargeTransactionThreshold { get; set; } = 50000m;
    public int InvoiceOverdueDays { get; set; } = 3;
}
