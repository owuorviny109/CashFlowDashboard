using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlowDashboard.Data.Repositories;

// Implementation of IForecastRepository using EF Core.
public class ForecastRepository : IForecastRepository
{
    private readonly CashFlowDbContext _context;

    public ForecastRepository(CashFlowDbContext context)
    {
        _context = context;
    }

    // Retrieves a forecast scenario by ID, including its collection of data points.
    // Eagerly loads DataPoints.
    public async Task<ForecastScenario?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Forecasts
            .Include(f => f.DataPoints)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    // Retrieves all scenarios currently marked as active.
    public async Task<IReadOnlyList<ForecastScenario>> GetActiveScenariosAsync(CancellationToken ct = default)
    {
        return await _context.Forecasts
            .AsNoTracking()
            .Where(f => f.IsActive)
            .OrderByDescending(f => f.GeneratedAt)
            .ToListAsync(ct);
    }

    // Adds a new forecast scenario and its data points to the database.
    public async Task AddAsync(ForecastScenario forecast, CancellationToken ct = default)
    {
        await _context.Forecasts.AddAsync(forecast, ct);
        await _context.SaveChangesAsync(ct);
    }

    // Updates an existing scenario.
    public async Task UpdateAsync(ForecastScenario forecast, CancellationToken ct = default)
    {
        _context.Forecasts.Update(forecast);
        await _context.SaveChangesAsync(ct);
    }

    // Deactivates (soft deletes) active scenarios older than the specified date.
    // Uses ExecuteUpdateAsync to modify multiple records efficiently.
    public async Task DeactivateOldScenariosAsync(DateTime olderThan, CancellationToken ct = default)
    {
        await _context.Forecasts
            .Where(f => f.GeneratedAt < olderThan && f.IsActive)
            .ExecuteUpdateAsync(calls => calls.SetProperty(f => f.IsActive, false), ct);
    }
}
