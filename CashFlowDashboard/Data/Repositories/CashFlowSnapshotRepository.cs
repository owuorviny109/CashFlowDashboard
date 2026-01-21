using CashFlowDashboard.Data.Repositories.Interfaces;
using CashFlowDashboard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlowDashboard.Data.Repositories;

// Implementation of ICashFlowSnapshotRepository using EF Core.
public class CashFlowSnapshotRepository : ICashFlowSnapshotRepository
{
    private readonly CashFlowDbContext _context;

    public CashFlowSnapshotRepository(CashFlowDbContext context)
    {
        _context = context;
    }

    // Retrieves a snapshot for a specific date.
    public async Task<CashFlowSnapshot?> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        return await _context.Snapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Date == date, ct);
    }

    // Retrieves a range of snapshots strictly between start and end dates.
    public async Task<IReadOnlyList<CashFlowSnapshot>> GetRangeAsync(DateOnly start, DateOnly end, CancellationToken ct = default)
    {
        return await _context.Snapshots
            .AsNoTracking()
            .Where(s => s.Date >= start && s.Date <= end)
            .OrderBy(s => s.Date)
            .ToListAsync(ct);
    }

    // Adds a new snapshot to the database.
    public async Task AddAsync(CashFlowSnapshot snapshot, CancellationToken ct = default)
    {
        await _context.Snapshots.AddAsync(snapshot, ct);
        await _context.SaveChangesAsync(ct);
    }
}
