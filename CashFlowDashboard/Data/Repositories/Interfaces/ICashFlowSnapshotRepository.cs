using CashFlowDashboard.Models.Entities;

namespace CashFlowDashboard.Data.Repositories.Interfaces;

// Interface for CashFlowSnapshot data access.
// Handles retrieval and creation of daily financial snapshots.
public interface ICashFlowSnapshotRepository
{
    // Retrieves a snapshot for a specific date.
    // Returns null if no snapshot exists for that day.
    Task<CashFlowSnapshot?> GetByDateAsync(DateOnly date, CancellationToken ct = default);

    // Retrieves a range of snapshots strictly between start and end dates (inclusive).
    // Ordered by date ascending.
    Task<IReadOnlyList<CashFlowSnapshot>> GetRangeAsync(DateOnly start, DateOnly end, CancellationToken ct = default);

    // Adds a new snapshot to the database.
    Task AddAsync(CashFlowSnapshot snapshot, CancellationToken ct = default);
}
