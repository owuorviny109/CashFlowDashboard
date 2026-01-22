namespace CashFlowDashboard.Configuration;

public class ForecastSettings
{
    public int DefaultHorizonDays { get; set; } = 30; // Forecast 30 days out by default
    public int HistoryDaysToAnalyze { get; set; } = 90; // Look back 90 days
}
