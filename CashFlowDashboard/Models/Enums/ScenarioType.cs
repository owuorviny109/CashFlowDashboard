namespace CashFlowDashboard.Models.Enums;

// Defines the type of forecast scenario based on assumptions and projections.
public enum ScenarioType
{
    // Baseline forecast using historical trends with no adjustments.
    BaseCase = 0,

    // Optimistic forecast assuming favorable conditions (higher revenue, lower costs).
    Optimistic = 1,

    // Pessimistic forecast assuming unfavorable conditions (lower revenue, higher costs).
    Pessimistic = 2,

    // Custom forecast with user-defined assumptions and parameters.
    Custom = 3
}
