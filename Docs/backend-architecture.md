# CashFlowDashboard: Backend Architecture Specification

**Status:** Design Specification  
**Principles:** CAP Theorem, SOLID, Domain-Driven Design, Separation of Concerns

---

## Executive Summary

This document specifies the production-grade backend architecture for CashFlowDashboard, designed to support:
- **Real-time cash flow analytics** with time-series forecasting
- **Rule-based alert generation** with configurable thresholds
- **Transactional data integrity** with ACID guarantees
- **Horizontal scalability** for future growth (post-MVP)
- **Testability** and **maintainability** through clean architecture

---

## Architecture Overview

### Layered Architecture (Vertical Slice with Horizontal Concerns)

```
┌─────────────────────────────────────────────────────────┐
│                  Presentation Layer                     │
│         (Controllers, ViewModels, Razor Views)          │
└─────────────────────────────────────────────────────────┘
                          ▼
┌─────────────────────────────────────────────────────────┐
│                   Service Layer                         │
│  (Business Logic, Forecasting Engine, Alert Rules)      │
└─────────────────────────────────────────────────────────┘
                          ▼
┌─────────────────────────────────────────────────────────┐
│                 Repository Layer                        │
│          (Data Access Abstraction, EF Core)             │
└─────────────────────────────────────────────────────────┘
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    Domain Layer                         │
│         (Entities, Value Objects, Enums)                │
└─────────────────────────────────────────────────────────┘
                          ▼
┌─────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                    │
│     (Database, Caching, Logging, Configuration)         │
└─────────────────────────────────────────────────────────┘
```

---

## 1. Domain Layer (Core Business Entities)

### 1.1 Transaction Entity
Represents a single cash flow event (income or expense).

**Properties:**
- `Id` (Guid): Primary key
- `Date` (DateTime): Transaction timestamp (indexed)
- `Amount` (decimal): Monetary value (precision: 18,2)
- `Type` (enum: Income, Expense): Direction of cash flow
- `Category` (string): Business category (e.g., "Sales", "Software", "Operations")
- `Description` (string): Human-readable description
- `ReferenceId` (string?): External invoice/receipt ID
- `IsRecurring` (bool): Indicates if this is a recurring transaction
- `RecurrencePattern` (enum?): Daily, Weekly, Monthly, Quarterly, Annually
- `CreatedAt` (DateTime): Audit timestamp
- `ModifiedAt` (DateTime?): Last update timestamp

**Business Rules:**
- Amount must be > 0
- Historical transactions (older than 90 days) are immutable
- Future-dated transactions (> 1 year) require approval flag

---

### 1.2 Alert Entity
Represents a system-generated or manual alert/notification.

**Properties:**
- `Id` (Guid): Primary key
- `Severity` (enum: Critical, Warning, Info, Success): Alert priority
- `Title` (string): Short headline
- `Message` (string): Detailed explanation
- `Category` (enum: CashFlow, Invoice, Forecast, Security, System): Alert domain
- `Status` (enum: Unread, Read, Dismissed, Resolved): User interaction state
- `GeneratedAt` (DateTime): Creation timestamp (indexed)
- `TriggeredBy` (string?): Rule identifier or user action
- `RelatedEntityId` (Guid?): FK to Transaction/Forecast if applicable
- `ActionUrl` (string?): Deep link to related page
- `ExpiresAt` (DateTime?): Auto-dismiss timestamp

**Business Rules:**
- Critical alerts cannot be auto-dismissed
- Alerts older than 30 days are archived
- Duplicate detection: Same title + category within 1 hour = suppress

---

### 1.3 CashFlowSnapshot Entity
Immutable point-in-time aggregate of account state (daily rollup).

**Properties:**
- `Id` (Guid): Primary key
- `Date` (DateOnly): Snapshot date (indexed, unique)
- `OpeningBalance` (decimal): Cash at start of day
- `TotalIncome` (decimal): Sum of income transactions
- `TotalExpenses` (decimal): Sum of expense transactions
- `ClosingBalance` (decimal): Computed: Opening + Income - Expenses
- `NetCashFlow` (decimal): Computed: Income - Expenses
- `TransactionCount` (int): Number of transactions that day
- `ComputedAt` (DateTime): Snapshot generation timestamp

**Business Rules:**
- Generated via daily background job
- Immutable after creation (append-only)
- Used for historical trend analysis

---

### 1.4 ForecastScenario Entity
Stores projected cash flow under different assumptions.

**Properties:**
- `Id` (Guid): Primary key
- `Name` (string): e.g., "Base Case", "Optimistic", "Pessimistic"
- `ScenarioType` (enum: BaseCase, Optimistic, Pessimistic, Custom): Preset or user-defined
- `StartDate` (DateOnly): Forecast horizon start
- `EndDate` (DateOnly): Forecast horizon end
- `Assumptions` (JSON): Serialized parameters (growth rate, churn, seasonality)
- `DataPoints` (List<ForecastDataPoint>): Time-series projection
- `ConfidenceLevel` (decimal): Statistical confidence (0.0 to 1.0)
- `GeneratedAt` (DateTime): Computation timestamp
- `IsActive` (bool): Currently displayed in UI

**Business Rules:**
- Maximum 3 active scenarios at once
- Forecasts older than 7 days trigger re-computation warning

---

### 1.5 ForecastDataPoint (Value Object)
Individual point in forecast time series.

**Properties:**
- `Date` (DateOnly): Forecast date
- `ProjectedBalance` (decimal): Predicted cash balance
- `LowerBound` (decimal): Pessimistic estimate
- `UpperBound` (decimal): Optimistic estimate
- `Confidence` (decimal): Prediction confidence

---

## 2. Repository Layer (Data Access Abstraction)

### Interfaces (Contracts)

```csharp
public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetRecentAsync(int count, CancellationToken ct = default);
    Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    Task UpdateAsync(Transaction transaction, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

public interface IAlertRepository
{
    Task<Alert?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Alert>> GetActiveAlertsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Alert>> GetBySeverityAsync(AlertSeverity severity, CancellationToken ct = default);
    Task<IReadOnlyList<Alert>> GetUnreadAsync(CancellationToken ct = default);
    Task AddAsync(Alert alert, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, AlertStatus newStatus, CancellationToken ct = default);
    Task MarkAllAsReadAsync(CancellationToken ct = default);
    Task ArchiveOldAlertsAsync(DateTime olderThan, CancellationToken ct = default);
}

public interface ICashFlowSnapshotRepository
{
    Task<CashFlowSnapshot?> GetByDateAsync(DateOnly date, CancellationToken ct = default);
    Task<IReadOnlyList<CashFlowSnapshot>> GetRangeAsync(DateOnly start, DateOnly end, CancellationToken ct = default);
    Task AddAsync(CashFlowSnapshot snapshot, CancellationToken ct = default);
}

public interface IForecastRepository
{
    Task<ForecastScenario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ForecastScenario>> GetActiveScenariosAsync(CancellationToken ct = default);
    Task AddAsync(ForecastScenario forecast, CancellationToken ct = default);
    Task UpdateAsync(ForecastScenario forecast, CancellationToken ct = default);
    Task DeactivateOldScenariosAsync(DateTime olderThan, CancellationToken ct = default);
}
```

### Implementation Notes
- EF Core with async/await patterns
- Query optimization with `.AsNoTracking()` for read-only operations
- Index on frequently queried fields (Date, Status, Severity)
- Use specification pattern for complex filtering (future enhancement)

---

## 3. Service Layer (Business Logic)

### 3.1 ITransactionService

**Responsibilities:**
- CRUD operations with validation
- Transaction aggregation (daily, weekly, monthly)
- Balance calculation at any point in time

**Key Methods:**
```csharp
public interface ITransactionService
{
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionCommand cmd, CancellationToken ct = default);
    Task<TransactionDto?> GetTransactionByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TransactionDto>> GetRecentTransactionsAsync(int count, CancellationToken ct = default);
    Task<TransactionSummaryDto> GetSummaryForPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default);
    Task UpdateTransactionAsync(Guid id, UpdateTransactionCommand cmd, CancellationToken ct = default);
    Task DeleteTransactionAsync(Guid id, CancellationToken ct = default);
}
```

**TransactionSummaryDto:**
- TotalIncome
- TotalExpenses
- NetCashFlow
- TransactionCount
- AverageTransactionSize

---

### 3.2 IAlertService

**Responsibilities:**
- Alert lifecycle management
- Rule-based alert generation
- Deduplication logic

**Key Methods:**
```csharp
public interface IAlertService
{
    Task<AlertDto> CreateManualAlertAsync(CreateAlertCommand cmd, CancellationToken ct = default);
    Task<IReadOnlyList<AlertDto>> GetActiveAlertsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<AlertDto>> GetAlertsBySeverityAsync(AlertSeverity severity, CancellationToken ct = default);
    Task MarkAsReadAsync(Guid alertId, CancellationToken ct = default);
    Task DismissAlertAsync(Guid alertId, CancellationToken ct = default);
    Task MarkAllAsReadAsync(CancellationToken ct = default);
    
    // Background job method
    Task GenerateSystemAlertsAsync(CancellationToken ct = default);
}
```

**Alert Generation Rules (Evaluated in GenerateSystemAlertsAsync):**

1. **Low Balance Alert** (Critical)
   - Trigger: Current balance < $10,000
   - Message: "Cash balance below safety threshold"

2. **Overdue Invoice Alert** (Critical)
   - Trigger: Transaction with `Type=Income` and `Date < Today - 3 days`
   - Message: "Payment overdue from [Description]"

3. **Projected Shortfall Alert** (Warning)
   - Trigger: Forecast shows balance < threshold within next 30 days
   - Message: "Forecast indicates dip below minimum on [Date]"

4. **Large Transaction Alert** (Info)
   - Trigger: Single transaction > $50,000
   - Message: "Large transfer detected: [Amount]"

5. **Tax Deadline Alert** (Warning)
   - Trigger: Fixed calendar dates (quarterly tax payments)
   - Message: "Q[X] tax payment due on [Date]"

---

### 3.3 ICashFlowAnalyticsService

**Responsibilities:**
- Historical trend analysis
- Daily snapshot generation
- Statistical computations

**Key Methods:**
```csharp
public interface ICashFlowAnalyticsService
{
    Task<CashFlowTrendDto> GetTrendForPeriodAsync(DateTime start, DateTime end, CancellationToken ct = default);
    Task<decimal> GetCurrentBalanceAsync(CancellationToken ct = default);
    Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct = default);
    Task<GrowthMetricsDto> GetGrowthMetricsAsync(CancellationToken ct = default);
    
    // Background job method
    Task GenerateDailySnapshotAsync(DateOnly date, CancellationToken ct = default);
}
```

**CashFlowTrendDto:**
- DataPoints: List of (Date, Balance, Income, Expenses)
- TrendDirection: enum (Upward, Downward, Stable)
- GrowthRate: Percentage change over period

**GrowthMetricsDto:**
- CurrentBalance
- BalanceChangePercent (30-day)
- NetCashFlow30Day
- BurnRate (monthly average expenses)

---

### 3.4 IForecastService (Core Forecasting Engine)

**Responsibilities:**
- Time-series projection using statistical methods
- Scenario modeling
- Confidence interval calculation

**Key Methods:**
```csharp
public interface IForecastService
{
    Task<ForecastScenarioDto> GenerateBaseCaseForecastAsync(int daysAhead, CancellationToken ct = default);
    Task<ForecastScenarioDto> GenerateOptimisticForecastAsync(int daysAhead, CancellationToken ct = default);
    Task<ForecastScenarioDto> GeneratePessimisticForecastAsync(int daysAhead, CancellationToken ct = default);
    Task<ForecastScenarioDto> GenerateCustomForecastAsync(ForecastAssumptions assumptions, CancellationToken ct = default);
    Task<IReadOnlyList<ForecastScenarioDto>> GetActiveForecastsAsync(CancellationToken ct = default);
}
```

**Forecasting Algorithm (Base Case):**

1. **Data Collection**
   - Fetch last 90 days of CashFlowSnapshots
   - Identify recurring transactions

2. **Trend Analysis**
   - Compute moving average (7-day, 30-day)
   - Calculate linear regression coefficients
   - Detect seasonality (day-of-week patterns)

3. **Projection Formula (Simplified Linear Model):**
   ```
   Balance(t) = CurrentBalance + (AvgDailyNetFlow × t) + RecurringIncome(t) - RecurringExpenses(t)
   ```

4. **Confidence Bounds**
   - Standard deviation of historical daily cash flow
   - Upper Bound = Projection + (2 × StdDev)
   - Lower Bound = Projection - (2 × StdDev)

5. **Scenario Adjustments**
   - **Optimistic:** AvgDailyNetFlow × 1.2, RecurringIncome × 1.1
   - **Pessimistic:** AvgDailyNetFlow × 0.8, RecurringExpenses × 1.1

**Advanced Forecasting (Future Enhancement):**
- Exponential smoothing (Holt-Winters)
- ARIMA models for seasonality
- Monte Carlo simulation for risk scenarios

---

## 4. Data Transfer Objects (DTOs)

### DashboardViewModel (Presentation)
```csharp
public sealed class DashboardViewModel
{
    public decimal CurrentBalance { get; init; }
    public decimal BalanceChangePercent { get; init; }
    public decimal NetCashFlow30Day { get; init; }
    public decimal ForecastGrowth60Day { get; init; }
    public int ActiveAlertCount { get; init; }
    
    public CashFlowChartData ChartData { get; init; } = null!;
    public IReadOnlyList<RecentAlertDto> RecentAlerts { get; init; } = Array.Empty<RecentAlertDto>();
}

public sealed class CashFlowChartData
{
    public IReadOnlyList<ChartDataPoint> Historical { get; init; } = Array.Empty<ChartDataPoint>();
    public IReadOnlyList<ChartDataPoint> Projected { get; init; } = Array.Empty<ChartDataPoint>();
}

public sealed class ChartDataPoint
{
    public string Date { get; init; } = string.Empty; // ISO 8601 format
    public decimal Balance { get; init; }
    public decimal? Income { get; init; }
    public decimal? Expenses { get; init; }
}
```

### AlertsViewModel
```csharp
public sealed class AlertsViewModel
{
    public string PageTitle { get; init; } = "System Alerts";
    public IReadOnlyList<AlertDto> TodayAlerts { get; init; } = Array.Empty<AlertDto>();
    public IReadOnlyList<AlertDto> YesterdayAlerts { get; init; } = Array.Empty<AlertDto>();
    public IReadOnlyList<AlertDto> OlderAlerts { get; init; } = Array.Empty<AlertDto>();
    public int UnreadCount { get; init; }
}

public sealed class AlertDto
{
    public Guid Id { get; init; }
    public string Severity { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string TimeAgo { get; init; } = string.Empty; // e.g., "2 hours ago"
    public string? ActionUrl { get; init; }
    public string Status { get; init; } = string.Empty;
}
```

### TransactionsViewModel
```csharp
public sealed class TransactionsViewModel
{
    public string PageTitle { get; init; } = "Transactions";
    public IReadOnlyList<TransactionDto> Transactions { get; init; } = Array.Empty<TransactionDto>();
    public int TotalCount { get; init; }
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
}

public sealed class TransactionDto
{
    public Guid Id { get; init; }
    public string Date { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ReferenceId { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // "Income" or "Expense"
    public decimal Amount { get; init; }
    public string FormattedAmount { get; init; } = string.Empty; // e.g., "+$1,250.00"
}
```

### ForecastViewModel
```csharp
public sealed class ForecastViewModel
{
    public string PageTitle { get; init; } = "Cash Flow Forecast";
    public ForecastScenarioDto ActiveScenario { get; init; } = null!;
    public IReadOnlyList<ForecastScenarioDto> AllScenarios { get; init; } = Array.Empty<ForecastScenarioDto>();
    public ForecastInsightsDto Insights { get; init; } = null!;
}

public sealed class ForecastScenarioDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ScenarioType { get; init; } = string.Empty;
    public IReadOnlyList<ForecastDataPointDto> DataPoints { get; init; } = Array.Empty<ForecastDataPointDto>();
    public decimal ConfidenceLevel { get; init; }
    public decimal EndCashBalance { get; init; }
    public decimal ProjectedGrowth { get; init; }
}

public sealed class ForecastDataPointDto
{
    public string Date { get; init; } = string.Empty;
    public decimal ProjectedBalance { get; init; }
    public decimal LowerBound { get; init; }
    public decimal UpperBound { get; init; }
}

public sealed class ForecastInsightsDto
{
    public decimal EndCashBalance { get; init; }
    public decimal BurnRatePerMonth { get; init; }
    public IReadOnlyList<UpcomingEventDto> UpcomingEvents { get; init; } = Array.Empty<UpcomingEventDto>();
    public decimal GrowthRate { get; init; }
    public decimal ChurnRate { get; init; }
}

public sealed class UpcomingEventDto
{
    public string Title { get; init; } = string.Empty;
    public string Date { get; init; } = string.Empty;
    public decimal EstimatedAmount { get; init; }
    public string Severity { get; init; } = string.Empty; // "warning", "info"
}
```

---

## 5. Infrastructure Configuration

### 5.1 Database (Entity Framework Core)

**Provider:** SQLite (MVP), PostgreSQL (Production)

**DbContext:**
```csharp
public class CashFlowDbContext : DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<CashFlowSnapshot> Snapshots => Set<CashFlowSnapshot>();
    public DbSet<ForecastScenario> Forecasts => Set<ForecastScenario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => new { e.Date, e.Type });
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Alert configuration
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GeneratedAt);
            entity.HasIndex(e => new { e.Status, e.Severity });
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        // Snapshot configuration (unique date constraint)
        modelBuilder.Entity<CashFlowSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Date).IsUnique();
            entity.Property(e => e.OpeningBalance).HasPrecision(18, 2);
        });

        // Forecast configuration
        modelBuilder.Entity<ForecastScenario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsMany(e => e.DataPoints, dp =>
            {
                dp.Property(p => p.ProjectedBalance).HasPrecision(18, 2);
                dp.Property(p => p.LowerBound).HasPrecision(18, 2);
                dp.Property(p => p.UpperBound).HasPrecision(18, 2);
            });
        });
    }
}
```

**Connection String (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cashflow.db"
  }
}
```

---

### 5.2 Dependency Injection (Program.cs)

```csharp
// Database
builder.Services.AddDbContext<CashFlowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<ICashFlowSnapshotRepository, CashFlowSnapshotRepository>();
builder.Services.AddScoped<IForecastRepository, ForecastRepository>();

// Services
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<ICashFlowAnalyticsService, CashFlowAnalyticsService>();
builder.Services.AddScoped<IForecastService, ForecastService>();

// Options Pattern for Configuration
builder.Services.Configure<ForecastSettings>(builder.Configuration.GetSection("Forecast"));
builder.Services.Configure<AlertSettings>(builder.Configuration.GetSection("Alerts"));

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Caching (future enhancement)
builder.Services.AddMemoryCache();
```

---

### 5.3 Configuration (appsettings.json)

```json
{
  "Forecast": {
    "DefaultHorizonDays": 60,
    "HistoricalDataWindowDays": 90,
    "OptimisticGrowthMultiplier": 1.2,
    "PessimisticGrowthMultiplier": 0.8
  },
  "Alerts": {
    "LowBalanceThreshold": 10000,
    "LargeTransactionThreshold": 50000,
    "OverdueInvoiceDays": 3,
    "AutoArchiveAfterDays": 30
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

---

## 6. Data Seeding Strategy

### Development Seed Data (Startup)

```csharp
public static class DataSeeder
{
    public static async Task SeedAsync(CashFlowDbContext context)
    {
        if (await context.Transactions.AnyAsync())
            return; // Database already seeded

        var transactions = new List<Transaction>
        {
            // Income transactions
            new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(-25), Amount = 1250.00m, Type = TransactionType.Income, Category = "Sales", Description = "Client Payment - #INV002", ReferenceId = "TXN-8831", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(-10), Amount = 3500.00m, Type = TransactionType.Income, Category = "Sales", Description = "Contract Renewal", ReferenceId = "TXN-8840", CreatedAt = DateTime.UtcNow },
            
            // Expense transactions
            new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(-24), Amount = 49.00m, Type = TransactionType.Expense, Category = "Software", Description = "SaaS Subscription", ReferenceId = "TXN-8832", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(-21), Amount = 120.00m, Type = TransactionType.Expense, Category = "Operations", Description = "Office Supplies", ReferenceId = "TXN-8830", CreatedAt = DateTime.UtcNow },
        };

        await context.Transactions.AddRangeAsync(transactions);

        var alerts = new List<Alert>
        {
            new() { Id = Guid.NewGuid(), Severity = AlertSeverity.Critical, Title = "Overdue Invoice #4029", Message = "Payment was expected 3 days ago from Acme Corp. This is impacting cash flow projections for the current week.", Category = AlertCategory.Invoice, Status = AlertStatus.Unread, GeneratedAt = DateTime.UtcNow.AddHours(-2), TriggeredBy = "InvoiceOverdueRule" },
            new() { Id = Guid.NewGuid(), Severity = AlertSeverity.Warning, Title = "Projected Cash Shortfall", Message = "Forecast indicates a dip below minimum balance threshold on Oct 24th due to pending vendor payouts.", Category = AlertCategory.Forecast, Status = AlertStatus.Unread, GeneratedAt = DateTime.UtcNow.AddHours(-5), TriggeredBy = "ForecastShortfallRule" },
        };

        await context.Alerts.AddRangeAsync(alerts);
        await context.SaveChangesAsync();
    }
}

// Call in Program.cs after app.Build()
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CashFlowDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DataSeeder.SeedAsync(context);
}
```

---

## 7. Error Handling & Resilience

### Exception Handling Middleware

```csharp
app.UseExceptionHandler("/Home/Error");

// Custom error handling for API endpoints (future)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (ValidationException ex)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (NotFoundException ex)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        // Log exception
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
    }
});
```

### Service Layer Validation

```csharp
public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionCommand cmd, CancellationToken ct)
{
    // Validation
    if (cmd.Amount <= 0)
        throw new ValidationException("Transaction amount must be positive");
    
    if (cmd.Date > DateTime.Today.AddYears(1))
        throw new ValidationException("Cannot create transactions more than 1 year in the future");

    // Business logic
    var transaction = new Transaction
    {
        Id = Guid.NewGuid(),
        Date = cmd.Date,
        Amount = cmd.Amount,
        Type = cmd.Type,
        Category = cmd.Category,
        Description = cmd.Description,
        CreatedAt = DateTime.UtcNow
    };

    await _transactionRepository.AddAsync(transaction, ct);
    return MapToDto(transaction);
}
```

---

## 8. Performance Considerations

### Query Optimization
- Use `.AsNoTracking()` for read-only queries
- Implement pagination for large datasets (transactions list)
- Add database indexes on frequently filtered columns

### Caching Strategy (Future)
- Cache daily snapshots (1 hour TTL)
- Cache active forecasts (30 minutes TTL)
- Cache dashboard metrics (5 minutes TTL)

### Forecasting Performance
- Pre-compute forecasts via background job (daily at midnight)
- Store computed scenarios in database
- UI fetches pre-computed results (avoid real-time calculation)

---

## 9. Testing Strategy

### Unit Tests
- Service layer business logic (forecasting algorithms, alert rules)
- DTO mapping logic
- Validation rules

### Integration Tests
- Repository layer (database operations)
- Full service workflows (create transaction → generate snapshot → trigger alert)

### End-to-End Tests
- Controller → Service → Repository flow
- View rendering with real data

---

## 10. Migration Path (Current State → Production)

### Phase 1: Core Domain (2-3 hours)
1. ✅ Create Entity classes (Transaction, Alert, Snapshot, Forecast)
2. ✅ Define enums (TransactionType, AlertSeverity, etc.)
3. ✅ Setup DbContext with configurations
4. ✅ Create initial migration
5. ✅ Implement data seeding

### Phase 2: Data Access (1-2 hours)
1. ✅ Implement repository interfaces
2. ✅ Create EF Core repository implementations
3. ✅ Add database indexes

### Phase 3: Business Logic (3-4 hours)
1. ✅ Implement TransactionService
2. ✅ Implement AlertService with rule engine
3. ✅ Implement CashFlowAnalyticsService
4. ✅ Implement ForecastService (basic linear model)

### Phase 4: Presentation Integration (2-3 hours)
1. ✅ Update ViewModels with real properties
2. ✅ Update Controllers to call services
3. ✅ Update Razor Views to bind to ViewModel data (remove hardcoded values)
4. ✅ Test all routes

### Phase 5: Polish (1 hour)
1. ✅ Error handling
2. ✅ Logging
3. ✅ Configuration validation
4. ✅ Final testing

**Total Estimated Time:** 9-13 hours

---

## 11. Future Enhancements (Post-Hackathon)

- **REST API**: Add Web API controllers for mobile/SPA consumption
- **Authentication**: Implement ASP.NET Core Identity
- **Multi-Tenancy**: Support multiple user accounts
- **Bank Integration**: Plaid/Yodlee for automated transaction import
- **Advanced Forecasting**: ARIMA, exponential smoothing, ML models
- **Export Features**: PDF/Excel report generation
- **Real-Time Updates**: SignalR for live dashboard updates
- **Horizontal Scaling**: Distributed caching (Redis), message queues (RabbitMQ)

---

## Appendix A: Key Design Decisions

### Why SQLite for MVP?
- Zero configuration
- Portable (single file database)
- Sufficient for hackathon demo
- Easy migration to PostgreSQL/SQL Server later

### Why Repository Pattern?
- Decouples business logic from data access
- Enables unit testing with mock repositories
- Supports future migration to different ORMs or databases

### Why Separate Service Layer?
- Business logic isolated from Controllers (thin controllers)
- Reusable across different presentation layers (MVC, API, CLI)
- Single Responsibility Principle

### Why Pre-Computed Forecasts?
- Avoid expensive calculations on every page load
- Better user experience (instant loading)
- Allows complex forecasting algorithms without timeout risk

---

**End of Specification**

This architecture provides a solid foundation for a production-grade financial application while remaining achievable within hackathon constraints. The design prioritizes correctness, maintainability, and future scalability.
