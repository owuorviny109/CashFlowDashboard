using CashFlowDashboard.Models.Entities;

namespace CashFlowDashboard.Data.Repositories.Interfaces;

// Interface for ForecastScenario data access.
// Manages storage and retrieval of financial forecast scenarios and their data points.
public interface IForecastRepository
{
    // Retrieves a forecast scenario by ID, including its collection of data points.
    Task<ForecastScenario?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Retrieves all scenarios currently marked as active.
    // Used for displaying comparison charts on the dashboard.
    Task<IReadOnlyList<ForecastScenario>> GetActiveScenariosAsync(CancellationToken ct = default);

    // Adds a new forecast scenario and its data points to the database.
    Task AddAsync(ForecastScenario forecast, CancellationToken ct = default);

    // Updates an existing scenario.
    Task UpdateAsync(ForecastScenario forecast, CancellationToken ct = default);

    // Deactivates (soft deletes) active scenarios older than the specified date.
    // Ensures we don't display stale forecasts.
    Task DeactivateOldScenariosAsync(DateTime olderThan, CancellationToken ct = default);
}
