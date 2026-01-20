using System.ComponentModel.DataAnnotations;

namespace CashFlowDashboard.Models.ValueObjects;

// Immutable value object representing a single point in a forecast time series.
// Contains projected balance with upper/lower confidence bounds.
public sealed record ForecastDataPoint
{
    // Date for this forecast data point.
    [Required]
    public DateOnly Date { get; init; }

    // Projected cash balance for this date.
    [Required]
    public decimal ProjectedBalance { get; init; }

    // Lower bound of confidence interval (pessimistic estimate).
    // Typically calculated as: Projection - (2 × StandardDeviation)
    [Required]
    public decimal LowerBound { get; init; }

    // Upper bound of confidence interval (optimistic estimate).
    // Typically calculated as: Projection + (2 × StandardDeviation)
    [Required]
    public decimal UpperBound { get; init; }

    // Confidence level for this specific data point (0.0 to 1.0).
    // Decreases as forecast extends further into the future.
    [Required]
    [Range(0.0, 1.0)]
    public decimal Confidence { get; init; }
}
