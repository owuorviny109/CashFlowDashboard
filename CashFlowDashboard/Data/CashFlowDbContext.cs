using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlowDashboard.Data;

// Entity Framework Core database context for CashFlowDashboard.
// Provides access to all domain entities and configures database mappings.
public sealed class CashFlowDbContext : DbContext
{
    public CashFlowDbContext(DbContextOptions<CashFlowDbContext> options)
        : base(options)
    {
    }

    // Transactions table (income and expense records).
    public DbSet<Transaction> Transactions => Set<Transaction>();

    // Alerts table (system-generated and manual notifications).
    public DbSet<Alert> Alerts => Set<Alert>();

    // Cash flow snapshots table (daily aggregates for trend analysis).
    public DbSet<CashFlowSnapshot> Snapshots => Set<CashFlowSnapshot>();

    // Forecast scenarios table (time-series projections).
    public DbSet<ForecastScenario> Forecasts => Set<ForecastScenario>();

    // Application settings table (alert thresholds, forecast intervals).
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Indexes for query performance
            entity.HasIndex(e => e.Date)
                .HasDatabaseName("IX_Transactions_Date");
            
            entity.HasIndex(e => new { e.Date, e.Type })
                .HasDatabaseName("IX_Transactions_Date_Type");
            
            // Decimal precision
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);
            
            // String length constraints
            entity.Property(e => e.Category)
                .HasMaxLength(100);
            
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            
            entity.Property(e => e.ReferenceId)
                .HasMaxLength(100);
        });

        // Configure Alert entity
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Indexes for filtering
            entity.HasIndex(e => e.GeneratedAt)
                .HasDatabaseName("IX_Alerts_GeneratedAt");
            
            entity.HasIndex(e => new { e.Status, e.Severity })
                .HasDatabaseName("IX_Alerts_Status_Severity");
            
            // String length constraints
            entity.Property(e => e.Title)
                .HasMaxLength(200);
            
            entity.Property(e => e.Message)
                .HasMaxLength(2000);
            
            entity.Property(e => e.TriggeredBy)
                .HasMaxLength(100);
            
            entity.Property(e => e.ActionUrl)
                .HasMaxLength(500);
        });

        // Configure CashFlowSnapshot entity
        modelBuilder.Entity<CashFlowSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Unique constraint: one snapshot per date
            entity.HasIndex(e => e.Date)
                .IsUnique()
                .HasDatabaseName("IX_Snapshots_Date_Unique");
            
            // Decimal precision for all balance fields
            entity.Property(e => e.OpeningBalance)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.TotalIncome)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.TotalExpenses)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.ClosingBalance)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.NetCashFlow)
                .HasPrecision(18, 2);
        });

        // Configure ForecastScenario entity
        modelBuilder.Entity<ForecastScenario>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // String length constraints
            entity.Property(e => e.Name)
                .HasMaxLength(100);
            
            entity.Property(e => e.ConfidenceLevel)
                .HasPrecision(5, 4); // 0.0000 to 1.0000
            
            // Configure owned collection (ForecastDataPoint)
            entity.OwnsMany(e => e.DataPoints, dataPoint =>
            {
                dataPoint.Property(p => p.ProjectedBalance)
                    .HasPrecision(18, 2);
                
                dataPoint.Property(p => p.LowerBound)
                    .HasPrecision(18, 2);
                
                dataPoint.Property(p => p.UpperBound)
                    .HasPrecision(18, 2);
                
                dataPoint.Property(p => p.Confidence)
                    .HasPrecision(5, 4);
            });
        });
    }
}
