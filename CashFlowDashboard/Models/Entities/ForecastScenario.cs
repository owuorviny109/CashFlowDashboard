using System.ComponentModel.DataAnnotations;
using CashFlowDashboard.Models.Enums;
using CashFlowDashboard.Models.ValueObjects;

namespace CashFlowDashboard.Models.Entities;

// Represents a projected cash flow scenario with time-series forecasts.
// Stores assumptions, data points, and confidence levels.
public sealed class ForecastScenario
{
    // Unique identifier for the forecast scenario.
    [Key]
    public Guid Id { get; set; }

    // Human-readable name for the scenario.
    // Examples: "Base Case", "Q4 Optimistic", "Conservative Growth"
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    // Type of scenario (BaseCase, Optimistic, Pessimistic, Custom).
    [Required]
    public ScenarioType ScenarioType { get; set; }

    // First date in the forecast horizon.
    [Required]
    public DateOnly StartDate { get; set; }

    // Last date in the forecast horizon.
    [Required]
    public DateOnly EndDate { get; set; }

    // JSON-serialized assumptions used for this forecast.
    // Example: { "growthRate": 0.05, "churnRate": 0.012, "seasonalityFactor": 1.1 }
    [Required]
    public string Assumptions { get; set; } = "{}";

    // Collection of time-series data points for the projection.
    // Owned entity collection (EF Core).
    public List<ForecastDataPoint> DataPoints { get; set; } = new();

    // Statistical confidence level (0.0 to 1.0).
    // Based on data quality and variance in historical trends.
    [Required]
    [Range(0.0, 1.0)]
    public decimal ConfidenceLevel { get; set; }

    // Timestamp when this forecast was generated.
    [Required]
    public DateTime GeneratedAt { get; set; }

    // Indicates whether this scenario is currently active/displayed.
    // Maximum 3 active scenarios allowed at once.
    [Required]
    public bool IsActive { get; set; }
}
