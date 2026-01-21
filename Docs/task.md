# CashFlowDashboard: Granular Development Roadmap

## Phase 1: Foundation & Architecture
- [x] **Project Initialization**
    - [x] Verify ASP.NET Core MVC project structure
    - [x] Add UI Mockups (`dashboard`, `alerts`, `transaction`, `financial`)
    - [x] Create Project Documentation & Roadmap (`task.md`)
    - [x] Establish Project Guardrails & Workflows (`setup_guardrails`, `create_adr`, `vertical_slice`)
    - [x] Remove default Bootstrap links from `_Layout.cshtml`
    - [x] Remove default `site.css` and `bootstrap` folder from `wwwroot`
    - [x] Add Tailwind CDN script to `_Layout.cshtml` `<head>`

- [x] **Core Layout (Master Template)**
    - [x] Extract **Head Section** (Fonts: Manrope, Material Symbols)
    - [x] Extract **Sidebar Component** (Navigation Links)
    - [x] Extract **Top Header Component** (Search, User Profile)
    - [x] Implement `@RenderBody()` main content area
    - [x] Verify Mobile Responsive Menu Toggle logic (Layout structure ready)

## Phase 2: Feature Views Migration

### Module A: Dashboard (Home)
- [x] **Setup**
    - [x] Locate `Views/Home/Index.cshtml`
- [x] **Content Migration**
    - [x] Copy **KPI Cards** (Total Balance, Income, Expenses)
    - [x] Copy **Chart Containers** (Cash Flow Trend)
    - [x] Copy **Recent Transactions** Widget
- [x] **Cleanup**
    - [x] Remove default "Welcome" text from template

### Module B: System Alerts
- [x] **Controller Setup**
    - [x] Create `Controllers/AlertsController.cs`
    - [x] Add `Index()` action method
- [x] **View Implementation**
    - [x] Create folder `Views/Alerts`
    - [x] Create `Views/Alerts/Index.cshtml`
    - [x] Copy **Filter Toolbar** (All, Critical, Warnings)
    - [x] Copy **Alert List Items** (Critical, Warning, Info, Success)

### Module C: Transactions
- [x] **Controller Setup**
    - [x] Create `Controllers/TransactionsController.cs`
    - [x] Add `Index()` action method
- [x] **View Implementation**
    - [x] Create folder `Views/Transactions`
    - [x] Create `Views/Transactions/Index.cshtml`
    - [x] Copy **Transaction Table** (Headers, Rows, Status Badges)
    - [x] Copy **Pagination Controls**

### Module D: Financial Forecast
- [x] **Controller Setup**
    - [x] Create `Controllers/ForecastController.cs`
    - [x] Add `Index()` action method
- [x] **View Implementation**
    - [x] Create folder `Views/Forecast`
    - [x] Create `Views/Forecast/Index.cshtml`
    - [x] Copy **Forecast Chart** container
    - [x] Copy **Prediction Summary** cards

## Phase 3: Quality Assurance (Frontend)
- [x] **Navigation Verification**
    - [x] Test Sidebar "Dashboard" link -> `/`
    - [x] Test Sidebar "Transactions" link -> `/Transactions`
    - [x] Test Sidebar "Forecast" link -> `/Forecast`
    - [x] Test Sidebar "System Alerts" link -> `/Alerts`
- [ ] **Visual Verification**
    - [ ] Check Mobile View (Hamburger menu, stacking)
    - [ ] Check Desktop View (Sidebar visibility)

---

# BACKEND IMPLEMENTATION

## Phase 4: Domain Layer (Core Entity Models)

### 4.1 Create Domain Models Directory Structure
- [ ] **Folder Setup**
    - [ ] Create `CashFlowDashboard/Models/Entities/` directory
    - [ ] Create `CashFlowDashboard/Models/Enums/` directory
    - [ ] Create `CashFlowDashboard/Models/ValueObjects/` directory

### 4.2 Define Core Enumerations
- [ ] **4.2.1 Transaction Enums**
    - [ ] Create `Models/Enums/TransactionType.cs`
        - [ ] Add enum values: `Income`, `Expense`
    - [ ] Create `Models/Enums/RecurrencePattern.cs`
        - [ ] Add enum values: `None`, `Daily`, `Weekly`, `Monthly`, `Quarterly`, `Annually`

- [ ] **4.2.2 Alert Enums**
    - [ ] Create `Models/Enums/AlertSeverity.cs`
        - [ ] Add enum values: `Critical`, `Warning`, `Info`, `Success`
    - [ ] Create `Models/Enums/AlertStatus.cs`
        - [ ] Add enum values: `Unread`, `Read`, `Dismissed`, `Resolved`
    - [ ] Create `Models/Enums/AlertCategory.cs`
        - [ ] Add enum values: `CashFlow`, `Invoice`, `Forecast`, `Security`, `System`

- [ ] **4.2.3 Forecast Enums**
    - [ ] Create `Models/Enums/ScenarioType.cs`
        - [ ] Add enum values: `BaseCase`, `Optimistic`, `Pessimistic`, `Custom`

### 4.3 Implement Core Entity Models
- [ ] **4.3.1 Transaction Entity**
    - [ ] Create `Models/Entities/Transaction.cs`
        - [ ] Add `Id` (Guid, PK)
        - [ ] Add `Date` (DateTime, indexed)
        - [ ] Add `Amount` (decimal, precision 18,2)
        - [ ] Add `Type` (TransactionType enum)
        - [ ] Add `Category` (string, max 100 chars)
        - [ ] Add `Description` (string, max 500 chars)
        - [ ] Add `ReferenceId` (string?, nullable)
        - [ ] Add `IsRecurring` (bool)
        - [ ] Add `RecurrencePattern` (RecurrencePattern? enum, nullable)
        - [ ] Add `CreatedAt` (DateTime, audit timestamp)
        - [ ] Add `ModifiedAt` (DateTime?, nullable, audit timestamp)
        - [ ] Implement validation attributes (Required, Range, StringLength)

- [ ] **4.3.2 Alert Entity**
    - [ ] Create `Models/Entities/Alert.cs`
        - [ ] Add `Id` (Guid, PK)
        - [ ] Add `Severity` (AlertSeverity enum)
        - [ ] Add `Title` (string, max 200 chars)
        - [ ] Add `Message` (string, max 2000 chars)
        - [ ] Add `Category` (AlertCategory enum)
        - [ ] Add `Status` (AlertStatus enum)
        - [ ] Add `GeneratedAt` (DateTime, indexed)
        - [ ] Add `TriggeredBy` (string?, nullable, rule identifier)
        - [ ] Add `RelatedEntityId` (Guid?, nullable, FK reference)
        - [ ] Add `ActionUrl` (string?, nullable, deep link)
        - [ ] Add `ExpiresAt` (DateTime?, nullable, auto-dismiss timestamp)

- [ ] **4.3.3 CashFlowSnapshot Entity**
    - [ ] Create `Models/Entities/CashFlowSnapshot.cs`
        - [ ] Add `Id` (Guid, PK)
        - [ ] Add `Date` (DateOnly, indexed, unique)
        - [ ] Add `OpeningBalance` (decimal, precision 18,2)
        - [ ] Add `TotalIncome` (decimal, precision 18,2)
        - [ ] Add `TotalExpenses` (decimal, precision 18,2)
        - [ ] Add `ClosingBalance` (decimal, computed property or field)
        - [ ] Add `NetCashFlow` (decimal, computed property or field)
        - [ ] Add `TransactionCount` (int)
        - [ ] Add `ComputedAt` (DateTime, timestamp)

- [ ] **4.3.4 ForecastScenario Entity**
    - [ ] Create `Models/Entities/ForecastScenario.cs`
        - [ ] Add `Id` (Guid, PK)
        - [ ] Add `Name` (string, max 100 chars)
        - [ ] Add `ScenarioType` (ScenarioType enum)
        - [ ] Add `StartDate` (DateOnly)
        - [ ] Add `EndDate` (DateOnly)
        - [ ] Add `Assumptions` (string, JSON serialized)
        - [ ] Add `DataPoints` (List<ForecastDataPoint>, owned collection)
        - [ ] Add `ConfidenceLevel` (decimal, 0.0 to 1.0)
        - [ ] Add `GeneratedAt` (DateTime)
        - [ ] Add `IsActive` (bool)

### 4.4 Implement Value Objects
- [ ] **4.4.1 ForecastDataPoint Value Object**
    - [ ] Create `Models/ValueObjects/ForecastDataPoint.cs`
        - [ ] Add `Date` (DateOnly)
        - [ ] Add `ProjectedBalance` (decimal, precision 18,2)
        - [ ] Add `LowerBound` (decimal, precision 18,2)
        - [ ] Add `UpperBound` (decimal, precision 18,2)
        - [ ] Add `Confidence` (decimal, 0.0 to 1.0)
        - [ ] Mark as `sealed record` (immutable)

### 4.5 Setup Entity Framework Core DbContext
- [ ] **4.5.1 Install EF Core NuGet Packages**
    - [ ] Run: `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`
    - [ ] Run: `dotnet add package Microsoft.EntityFrameworkCore.Design`
    - [ ] Run: `dotnet add package Microsoft.EntityFrameworkCore.Tools`

- [ ] **4.5.2 Create DbContext**
    - [ ] Create `Data/` directory
    - [ ] Create `Data/CashFlowDbContext.cs`
        - [ ] Inherit from `DbContext`
        - [ ] Add `DbSet<Transaction> Transactions` property
        - [ ] Add `DbSet<Alert> Alerts` property
        - [ ] Add `DbSet<CashFlowSnapshot> Snapshots` property
        - [ ] Add `DbSet<ForecastScenario> Forecasts` property
        - [ ] Override `OnModelCreating()` method

- [ ] **4.5.3 Configure Entity Mappings** (in `OnModelCreating`)
    - [ ] Configure `Transaction` entity
        - [ ] Set primary key on `Id`
        - [ ] Add index on `Date`
        - [ ] Add composite index on `(Date, Type)`
        - [ ] Set decimal precision for `Amount` (18,2)
        - [ ] Set max lengths for string properties
    - [ ] Configure `Alert` entity
        - [ ] Set primary key on `Id`
        - [ ] Add index on `GeneratedAt`
        - [ ] Add composite index on `(Status, Severity)`
        - [ ] Set max lengths for string properties
    - [ ] Configure `CashFlowSnapshot` entity
        - [ ] Set primary key on `Id`
        - [ ] Add unique index on `Date`
        - [ ] Set decimal precision for all balance fields (18,2)
    - [ ] Configure `ForecastScenario` entity
        - [ ] Set primary key on `Id`
        - [ ] Configure owned collection `DataPoints` (EF Core owned type)
        - [ ] Set decimal precision for `ConfidenceLevel`

### 4.6 Configure Database Connection
- [ ] **4.6.1 Update Configuration File**
    - [ ] Open `appsettings.json`
    - [ ] Add `ConnectionStrings` section
        - [ ] Add `DefaultConnection`: `"Data Source=cashflow.db"`
    - [ ] Add `Forecast` section with settings (DefaultHorizonDays, etc.)
    - [ ] Add `Alerts` section with thresholds (LowBalanceThreshold, etc.)

- [ ] **4.6.2 Register DbContext in DI Container**
    - [ ] Open `Program.cs`
    - [ ] Add `builder.Services.AddDbContext<CashFlowDbContext>()` before `builder.Build()`
    - [ ] Configure SQLite with connection string from config

### 4.7 Create Initial Database Migration
- [ ] **4.7.1 Generate Migration**
    - [ ] Run: `dotnet ef migrations add InitialCreate -o Data/Migrations`
    - [ ] Verify migration file created in `Data/Migrations/`
    - [ ] Review Up/Down migration scripts

- [ ] **4.7.2 Apply Migration**
    - [ ] Run: `dotnet ef database update`
    - [ ] Verify `cashflow.db` file created in project root
    - [ ] Inspect database schema using SQLite browser (optional)

### 4.8 Implement Data Seeding
- [ ] **4.8.1 Create Seed Data Class**
    - [ ] Create `Data/DataSeeder.cs`
        - [ ] Add static method `SeedAsync(CashFlowDbContext context)`
        - [ ] Check if database already has data (early return if seeded)
        - [ ] Create sample transactions (5-10 records: mix of income/expense)
        - [ ] Create sample alerts (3-5 records: different severities)
        - [ ] Call `context.SaveChangesAsync()`

- [ ] **4.8.2 Integrate Seeding in Startup**
    - [ ] Open `Program.cs`
    - [ ] After `app.Build()`, create scope
    - [ ] Get `CashFlowDbContext` from DI
    - [ ] Call `await context.Database.EnsureCreatedAsync()`
    - [ ] Call `await DataSeeder.SeedAsync(context)`

---

## Phase 5: Repository Layer (Data Access Abstraction)

### 5.1 Create Repository Directory Structure
- [x] **Folder Setup**
    - [x] Create `Data/Repositories/` directory
    - [x] Create `Data/Repositories/Interfaces/` directory

### 5.2 Define Repository Interfaces
- [x] **5.2.1 ITransactionRepository**
    - [x] Create `Data/Repositories/Interfaces/ITransactionRepository.cs`
        - [x] Add `Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<Transaction>> GetRecentAsync(int count, CancellationToken ct)`
        - [x] Add `Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct)`
        - [x] Add `Task AddAsync(Transaction transaction, CancellationToken ct)`
        - [x] Add `Task UpdateAsync(Transaction transaction, CancellationToken ct)`
        - [x] Add `Task DeleteAsync(Guid id, CancellationToken ct)`
        - [x] Add `Task<bool> ExistsAsync(Guid id, CancellationToken ct)`

- [x] **5.2.2 IAlertRepository**
    - [x] Create `Data/Repositories/Interfaces/IAlertRepository.cs`
        - [x] Add `Task<Alert?> GetByIdAsync(Guid id, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<Alert>> GetActiveAlertsAsync(CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<Alert>> GetBySeverityAsync(AlertSeverity severity, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<Alert>> GetUnreadAsync(CancellationToken ct)`
        - [x] Add `Task AddAsync(Alert alert, CancellationToken ct)`
        - [x] Add `Task UpdateStatusAsync(Guid id, AlertStatus newStatus, CancellationToken ct)`
        - [x] Add `Task MarkAllAsReadAsync(CancellationToken ct)`
        - [x] Add `Task ArchiveOldAlertsAsync(DateTime olderThan, CancellationToken ct)`

- [x] **5.2.3 ICashFlowSnapshotRepository**
    - [x] Create `Data/Repositories/Interfaces/ICashFlowSnapshotRepository.cs`
        - [x] Add `Task<CashFlowSnapshot?> GetByDateAsync(DateOnly date, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<CashFlowSnapshot>> GetRangeAsync(DateOnly start, DateOnly end, CancellationToken ct)`
        - [x] Add `Task AddAsync(CashFlowSnapshot snapshot, CancellationToken ct)`

- [x] **5.2.4 IForecastRepository**
    - [x] Create `Data/Repositories/Interfaces/IForecastRepository.cs`
        - [x] Add `Task<ForecastScenario?> GetByIdAsync(Guid id, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<ForecastScenario>> GetActiveScenariosAsync(CancellationToken ct)`
        - [x] Add `Task AddAsync(ForecastScenario forecast, CancellationToken ct)`
        - [x] Add `Task UpdateAsync(ForecastScenario forecast, CancellationToken ct)`
        - [x] Add `Task DeactivateOldScenariosAsync(DateTime olderThan, CancellationToken ct)`

### 5.3 Implement Repository Classes (EF Core)
- [x] **5.3.1 TransactionRepository**
    - [x] Create `Data/Repositories/TransactionRepository.cs`
        - [x] Inject `CashFlowDbContext` via constructor
        - [x] Implement `GetByIdAsync()` using `FindAsync()`
        - [x] Implement `GetByDateRangeAsync()` with `.Where().OrderByDescending()`
        - [x] Implement `GetRecentAsync()` with `.OrderByDescending().Take()`
        - [x] Implement `GetBalanceAtDateAsync()` (sum all transactions up to date)
        - [x] Implement `AddAsync()` using `AddAsync()` + `SaveChangesAsync()`
        - [x] Implement `UpdateAsync()` using `Update()` + `SaveChangesAsync()`
        - [x] Implement `DeleteAsync()` using `Remove()` + `SaveChangesAsync()`
        - [x] Implement `ExistsAsync()` using `AnyAsync()`
        - [x] Use `.AsNoTracking()` for read-only queries

- [x] **5.3.2 AlertRepository**
    - [x] Create `Data/Repositories/AlertRepository.cs`
        - [x] Inject `CashFlowDbContext` via constructor
        - [x] Implement `GetByIdAsync()` using `FindAsync()`
        - [x] Implement `GetActiveAlertsAsync()` (filter by Status != Resolved, Dismissed)
        - [x] Implement `GetBySeverityAsync()` with `.Where()`
        - [x] Implement `GetUnreadAsync()` (filter by Status == Unread)
        - [x] Implement `AddAsync()`
        - [x] Implement `UpdateStatusAsync()` (load entity, update Status, save)
        - [x] Implement `MarkAllAsReadAsync()` (ExecuteUpdateAsync on all unread)
        - [x] Implement `ArchiveOldAlertsAsync()` (update Status or delete)

- [x] **5.3.3 CashFlowSnapshotRepository**
    - [x] Create `Data/Repositories/CashFlowSnapshotRepository.cs`
        - [x] Inject `CashFlowDbContext` via constructor
        - [x] Implement `GetByDateAsync()` using `FirstOrDefaultAsync()`
        - [x] Implement `GetRangeAsync()` with `.Where().OrderBy()`
        - [x] Implement `AddAsync()`

- [x] **5.3.4 ForecastRepository**
    - [x] Create `Data/Repositories/ForecastRepository.cs`
        - [x] Inject `CashFlowDbContext` via constructor
        - [x] Implement `GetByIdAsync()` (include DataPoints collection)
        - [x] Implement `GetActiveScenariosAsync()` (filter by IsActive == true)
        - [x] Implement `AddAsync()`
        - [x] Implement `UpdateAsync()`
        - [x] Implement `DeactivateOldScenariosAsync()`

### 5.4 Register Repositories in DI Container
- [x] **Update Program.cs**
    - [x] Add `builder.Services.AddScoped<ITransactionRepository, TransactionRepository>()`
    - [x] Add `builder.Services.AddScoped<IAlertRepository, AlertRepository>()`
    - [x] Add `builder.Services.AddScoped<ICashFlowSnapshotRepository, CashFlowSnapshotRepository>()`
    - [x] Add `builder.Services.AddScoped<IForecastRepository, ForecastRepository>()`

---

## Phase 6: Service Layer (Business Logic & Forecasting Engine)

### 6.1 Create Service Directory Structure
- [ ] **Folder Setup**
    - [ ] Create `Services/` directory
    - [ ] Create `Services/Interfaces/` directory
    - [ ] Create `Services/DTOs/` directory

### 6.2 Define Data Transfer Objects (DTOs)
- [ ] **6.2.1 Core DTOs**
    - [ ] Create `Services/DTOs/TransactionDto.cs`
        - [ ] Add all display properties (Id, Date, Description, Category, Type, Amount, FormattedAmount)
    - [ ] Create `Services/DTOs/AlertDto.cs`
        - [ ] Add properties (Id, Severity, Title, Message, TimeAgo, Status, ActionUrl)
    - [ ] Create `Services/DTOs/ForecastDataPointDto.cs`
        - [ ] Add properties (Date, ProjectedBalance, LowerBound, UpperBound)
    - [ ] Create `Services/DTOs/ForecastScenarioDto.cs`
        - [ ] Add properties (Id, Name, ScenarioType, DataPoints, ConfidenceLevel, EndCashBalance)

- [ ] **6.2.2 Aggregate DTOs**
    - [ ] Create `Services/DTOs/TransactionSummaryDto.cs`
        - [ ] Add TotalIncome, TotalExpenses, NetCashFlow, TransactionCount, AverageTransactionSize
    - [ ] Create `Services/DTOs/CashFlowTrendDto.cs`
        - [ ] Add DataPoints list, TrendDirection enum, GrowthRate
    - [ ] Create `Services/DTOs/GrowthMetricsDto.cs`
        - [ ] Add CurrentBalance, BalanceChangePercent, NetCashFlow30Day, BurnRate
    - [ ] Create `Services/DTOs/ChartDataPoint.cs`
        - [ ] Add Date (string, ISO 8601), Balance, Income, Expenses (all nullable except Date)

- [ ] **6.2.3 Command DTOs** (for create/update operations)
    - [ ] Create `Services/DTOs/CreateTransactionCommand.cs`
        - [ ] Add Date, Amount, Type, Category, Description, IsRecurring, RecurrencePattern
    - [ ] Create `Services/DTOs/UpdateTransactionCommand.cs`
        - [ ] Add updatable fields (Amount, Category, Description)
    - [ ] Create `Services/DTOs/CreateAlertCommand.cs`
        - [ ] Add Severity, Title, Message, Category

### 6.3 Implement Service Interfaces
- [ ] **6.3.1 ITransactionService**
    - [ ] Create `Services/Interfaces/ITransactionService.cs`
        - [ ] Add `Task<TransactionDto> CreateTransactionAsync(CreateTransactionCommand cmd, CancellationToken ct)`
        - [ ] Add `Task<TransactionDto?> GetTransactionByIdAsync(Guid id, CancellationToken ct)`
        - [ ] Add `Task<IReadOnlyList<TransactionDto>> GetRecentTransactionsAsync(int count, CancellationToken ct)`
        - [ ] Add `Task<TransactionSummaryDto> GetSummaryForPeriodAsync(DateTime start, DateTime end, CancellationToken ct)`
        - [ ] Add `Task UpdateTransactionAsync(Guid id, UpdateTransactionCommand cmd, CancellationToken ct)`
        - [ ] Add `Task DeleteTransactionAsync(Guid id, CancellationToken ct)`

- [ ] **6.3.2 IAlertService**
    - [ ] Create `Services/Interfaces/IAlertService.cs`
        - [ ] Add `Task<AlertDto> CreateManualAlertAsync(CreateAlertCommand cmd, CancellationToken ct)`
        - [ ] Add `Task<IReadOnlyList<AlertDto>> GetActiveAlertsAsync(CancellationToken ct)`
        - [ ] Add `Task<IReadOnlyList<AlertDto>> GetAlertsBySeverityAsync(AlertSeverity severity, CancellationToken ct)`
        - [ ] Add `Task MarkAsReadAsync(Guid alertId, CancellationToken ct)`
        - [ ] Add `Task DismissAlertAsync(Guid alertId, CancellationToken ct)`
        - [ ] Add `Task MarkAllAsReadAsync(CancellationToken ct)`
        - [ ] Add `Task GenerateSystemAlertsAsync(CancellationToken ct)` (background job method)

- [ ] **6.3.3 ICashFlowAnalyticsService**
    - [ ] Create `Services/Interfaces/ICashFlowAnalyticsService.cs`
        - [ ] Add `Task<CashFlowTrendDto> GetTrendForPeriodAsync(DateTime start, DateTime end, CancellationToken ct)`
        - [ ] Add `Task<decimal> GetCurrentBalanceAsync(CancellationToken ct)`
        - [ ] Add `Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct)`
        - [ ] Add `Task<GrowthMetricsDto> GetGrowthMetricsAsync(CancellationToken ct)`
        - [ ] Add `Task GenerateDailySnapshotAsync(DateOnly date, CancellationToken ct)` (background job)

- [ ] **6.3.4 IForecastService**
    - [ ] Create `Services/Interfaces/IForecastService.cs`
        - [ ] Add `Task<ForecastScenarioDto> GenerateBaseCaseForecastAsync(int daysAhead, CancellationToken ct)`
        - [ ] Add `Task<ForecastScenarioDto> GenerateOptimisticForecastAsync(int daysAhead, CancellationToken ct)`
        - [ ] Add `Task<ForecastScenarioDto> GeneratePessimisticForecastAsync(int daysAhead, CancellationToken ct)`
        - [ ] Add `Task<IReadOnlyList<ForecastScenarioDto>> GetActiveForecastsAsync(CancellationToken ct)`

### 6.4 Implement TransactionService
- [ ] **Create `Services/TransactionService.cs`**
    - [ ] Inject `ITransactionRepository` and `ILogger<TransactionService>`
    - [ ] Implement `CreateTransactionAsync()`
        - [ ] Validate command (amount > 0, valid dates)
        - [ ] Map command to entity
        - [ ] Call repository AddAsync
        - [ ] Map entity to DTO and return
    - [ ] Implement `GetTransactionByIdAsync()`
        - [ ] Call repository GetByIdAsync
        - [ ] Map entity to DTO (handle null case)
    - [ ] Implement `GetRecentTransactionsAsync()`
        - [ ] Call repository GetRecentAsync
        - [ ] Map list to DTOs
    - [ ] Implement `GetSummaryForPeriodAsync()`
        - [ ] Call repository GetByDateRangeAsync
        - [ ] Compute aggregates (sum income, sum expenses, count, average)
        - [ ] Return summary DTO
    - [ ] Implement `UpdateTransactionAsync()`
        - [ ] Validate command
        - [ ] Load existing entity
        - [ ] Update properties
        - [ ] Call repository UpdateAsync
    - [ ] Implement `DeleteTransactionAsync()`
        - [ ] Call repository DeleteAsync

### 6.5 Implement AlertService (with Rule Engine)
- [ ] **Create `Services/AlertService.cs`**
    - [ ] Inject `IAlertRepository`, `ITransactionRepository`, `IForecastRepository`, `ILogger<AlertService>`
    - [ ] Inject `IOptions<AlertSettings>` for configuration
    - [ ] Implement `CreateManualAlertAsync()`
        - [ ] Map command to entity
        - [ ] Call repository AddAsync
    - [ ] Implement `GetActiveAlertsAsync()`
        - [ ] Call repository GetActiveAlertsAsync
        - [ ] Map to DTOs (compute TimeAgo strings)
    - [ ] Implement `GetAlertsBySeverityAsync()`
    - [ ] Implement `MarkAsReadAsync()` (update status to Read)
    - [ ] Implement `DismissAlertAsync()` (update status to Dismissed)
    - [ ] Implement `MarkAllAsReadAsync()`
    - [ ] **Implement `GenerateSystemAlertsAsync()` (Rule Engine)**
        - [ ] **Rule 1: Low Balance Alert**
            - [ ] Get current balance from transaction repository
            - [ ] If balance < threshold (from config), create Critical alert
        - [ ] **Rule 2: Overdue Invoice Alert**
            - [ ] Query transactions where Type=Income and Date < Today - 3 days
            - [ ] For each, check if alert already exists (deduplication)
            - [ ] Create Critical alert for new overdue invoices
        - [ ] **Rule 3: Projected Shortfall Alert**
            - [ ] Get active forecast scenarios
            - [ ] Check if any DataPoint in next 30 days has balance < threshold
            - [ ] Create Warning alert if shortfall detected
        - [ ] **Rule 4: Large Transaction Alert**
            - [ ] Query recent transactions with Amount > threshold
            - [ ] Create Info alert for each (if not already alerted)
        - [ ] Save all generated alerts in batch

### 6.6 Implement CashFlowAnalyticsService
- [ ] **Create `Services/CashFlowAnalyticsService.cs`**
    - [ ] Inject `ITransactionRepository`, `ICashFlowSnapshotRepository`, `ILogger<CashFlowAnalyticsService>`
    - [ ] Implement `GetCurrentBalanceAsync()`
        - [ ] Call repository GetBalanceAtDateAsync(DateTime.Today)
    - [ ] Implement `GetBalanceAtDateAsync()`
        - [ ] Call repository GetBalanceAtDateAsync(date)
    - [ ] Implement `GetTrendForPeriodAsync()`
        - [ ] Get snapshots from repository for date range
        - [ ] Map to trend data points
        - [ ] Calculate trend direction (compare first vs last)
        - [ ] Return trend DTO
    - [ ] Implement `GetGrowthMetricsAsync()`
        - [ ] Get current balance
        - [ ] Get balance 30 days ago
        - [ ] Calculate percentage change
        - [ ] Get net cash flow for last 30 days
        - [ ] Calculate burn rate (average monthly expenses)
        - [ ] Return metrics DTO
    - [ ] **Implement `GenerateDailySnapshotAsync()`**
        - [ ] Check if snapshot already exists for date
        - [ ] Get all transactions for the date
        - [ ] Get opening balance (closing balance from previous day)
        - [ ] Sum income and expenses for the day
        - [ ] Calculate closing balance
        - [ ] Create and save snapshot entity

### 6.7 Implement ForecastService (Forecasting Engine)
- [ ] **Create `Services/ForecastService.cs`**
    - [ ] Inject `ITransactionRepository`, `ICashFlowSnapshotRepository`, `IForecastRepository`, `ILogger<ForecastService>`
    - [ ] Inject `IOptions<ForecastSettings>` for configuration
    - [ ] **Implement Core Forecasting Logic (Private Helper Methods)**
        - [ ] Create `private async Task<ForecastScenario> GenerateForecastAsync(ScenarioType type, int daysAhead, decimal growthMultiplier)`
            - [ ] **Step 1: Data Collection**
                - [ ] Fetch last 90 days of snapshots
                - [ ] Fetch recurring transactions
            - [ ] **Step 2: Trend Analysis**
                - [ ] Compute 7-day moving average of net cash flow
                - [ ] Compute 30-day moving average
                - [ ] Calculate linear regression coefficients (slope, intercept)
                - [ ] Calculate standard deviation for confidence bounds
            - [ ] **Step 3: Projection**
                - [ ] Get current balance
                - [ ] For each day in forecast horizon:
                    - [ ] Project balance = CurrentBalance + (AvgDailyNetFlow × growthMultiplier × daysElapsed) + RecurringIncome - RecurringExpenses
                    - [ ] Calculate upper bound = Projection + (2 × StdDev)
                    - [ ] Calculate lower bound = Projection - (2 × StdDev)
                    - [ ] Create ForecastDataPoint with date, projected, upper, lower
            - [ ] **Step 4: Build Scenario**
                - [ ] Create ForecastScenario entity
                - [ ] Set Name, Type, StartDate, EndDate
                - [ ] Add DataPoints collection
                - [ ] Calculate ConfidenceLevel (based on data quality)
                - [ ] Set GeneratedAt, IsActive = true
            - [ ] Return scenario
    - [ ] Implement `GenerateBaseCaseForecastAsync()`
        - [ ] Call helper with ScenarioType.BaseCase, growthMultiplier = 1.0
        - [ ] Save to repository
        - [ ] Map to DTO and return
    - [ ] Implement `GenerateOptimisticForecastAsync()`
        - [ ] Call helper with ScenarioType.Optimistic, growthMultiplier = 1.2
        - [ ] Adjust recurring income +10%
        - [ ] Save and return
    - [ ] Implement `GeneratePessimisticForecastAsync()`
        - [ ] Call helper with ScenarioType.Pessimistic, growthMultiplier = 0.8
        - [ ] Adjust recurring expenses +10%
        - [ ] Save and return
    - [ ] Implement `GetActiveForecastsAsync()`
        - [ ] Call repository GetActiveScenariosAsync
        - [ ] Map to DTOs

### 6.8 Register Services in DI Container
- [ ] **Update Program.cs**
    - [ ] Add `builder.Services.AddScoped<ITransactionService, TransactionService>()`
    - [ ] Add `builder.Services.AddScoped<IAlertService, AlertService>()`
    - [ ] Add `builder.Services.AddScoped<ICashFlowAnalyticsService, CashFlowAnalyticsService>()`
    - [ ] Add `builder.Services.AddScoped<IForecastService, ForecastService>()`
    - [ ] Add `builder.Services.Configure<ForecastSettings>(builder.Configuration.GetSection("Forecast"))`
    - [ ] Add `builder.Services.Configure<AlertSettings>(builder.Configuration.GetSection("Alerts"))`

---

## Phase 7: Presentation Integration (Wire Controllers & Update Views)

### 7.1 Update ViewModels with Real Properties
- [ ] **7.1.1 Update DashboardViewModel**
    - [ ] Open `ViewModels/DashboardViewModel.cs`
    - [ ] Replace placeholder properties with:
        - [ ] `decimal CurrentBalance { get; init; }`
        - [ ] `decimal BalanceChangePercent { get; init; }`
        - [ ] `decimal NetCashFlow30Day { get; init; }`
        - [ ] `decimal ForecastGrowth60Day { get; init; }`
        - [ ] `int ActiveAlertCount { get; init; }`
        - [ ] `IReadOnlyList<ChartDataPoint> HistoricalChartData { get; init; }`
        - [ ] `IReadOnlyList<ChartDataPoint> ProjectedChartData { get; init; }`
        - [ ] `IReadOnlyList<AlertDto> RecentAlerts { get; init; }`

- [ ] **7.1.2 Update AlertsViewModel**
    - [ ] Open `ViewModels/AlertsViewModel.cs`
    - [ ] Add properties:
        - [ ] `IReadOnlyList<AlertDto> TodayAlerts { get; init; }`
        - [ ] `IReadOnlyList<AlertDto> YesterdayAlerts { get; init; }`
        - [ ] `IReadOnlyList<AlertDto> OlderAlerts { get; init; }`
        - [ ] `int UnreadCount { get; init; }`

- [ ] **7.1.3 Update TransactionsViewModel**
    - [ ] Open `ViewModels/TransactionsViewModel.cs`
    - [ ] Add properties:
        - [ ] `IReadOnlyList<TransactionDto> Transactions { get; init; }`
        - [ ] `int TotalCount { get; init; }`
        - [ ] `int CurrentPage { get; init; }`
        - [ ] `int PageSize { get; init; }`

- [ ] **7.1.4 Update ForecastViewModel**
    - [ ] Open `ViewModels/ForecastViewModel.cs`
    - [ ] Add properties:
        - [ ] `ForecastScenarioDto ActiveScenario { get; init; }`
        - [ ] `IReadOnlyList<ForecastScenarioDto> AllScenarios { get; init; }`
        - [ ] `decimal EndCashBalance { get; init; }`
        - [ ] `decimal BurnRatePerMonth { get; init; }`
        - [ ] `decimal GrowthRate { get; init; }`

### 7.2 Update Controllers to Call Services
- [ ] **7.2.1 Update HomeController (Dashboard)**
    - [ ] Open `Controllers/HomeController.cs`
    - [ ] Inject services: `ICashFlowAnalyticsService`, `IAlertService`, `IForecastService`
    - [ ] Update `Index()` action method:
        - [ ] Change signature to `async Task<IActionResult> Index(CancellationToken ct)`
        - [ ] Call `await _analyticsService.GetCurrentBalanceAsync(ct)`
        - [ ] Call `await _analyticsService.GetGrowthMetricsAsync(ct)`
        - [ ] Call `await _analyticsService.GetTrendForPeriodAsync()` (last 6 months)
        - [ ] Call `await _alertService.GetActiveAlertsAsync(ct)`
        - [ ] Call `await _forecastService.GetActiveForecastsAsync(ct)` (for forecast growth)
        - [ ] Populate `DashboardViewModel` with real data
        - [ ] Return `View(model)`

- [ ] **7.2.2 Update AlertsController**
    - [ ] Open `Controllers/AlertsController.cs`
    - [ ] Inject `IAlertService`
    - [ ] Update `Index()` action:
        - [ ] Change to async
        - [ ] Call `await _alertService.GetActiveAlertsAsync(ct)`
        - [ ] Group alerts by date (Today, Yesterday, Older)
        - [ ] Populate `AlertsViewModel`
        - [ ] Return `View(model)`
    - [ ] Add `[HttpPost] MarkAsRead(Guid id)` action
        - [ ] Call service method
        - [ ] Redirect to Index
    - [ ] Add `[HttpPost] MarkAllAsRead()` action

- [ ] **7.2.3 Update TransactionsController**
    - [ ] Open `Controllers/TransactionsController.cs`
    - [ ] Inject `ITransactionService`
    - [ ] Update `Index()` action:
        - [ ] Add pagination parameters (page, pageSize)
        - [ ] Call `await _transactionService.GetRecentTransactionsAsync()`
        - [ ] Populate `TransactionsViewModel`
        - [ ] Return `View(model)`
    - [ ] Add `[HttpPost] Create(CreateTransactionCommand cmd)` action
        - [ ] Validate model
        - [ ] Call service CreateTransactionAsync
        - [ ] Redirect to Index
    - [ ] Add `[HttpPost] Delete(Guid id)` action

- [ ] **7.2.4 Update ForecastController**
    - [ ] Open `Controllers/ForecastController.cs`
    - [ ] Inject `IForecastService`, `ICashFlowAnalyticsService`
    - [ ] Update `Index()` action:
        - [ ] Call `await _forecastService.GetActiveForecastsAsync(ct)`
        - [ ] Call `await _analyticsService.GetGrowthMetricsAsync(ct)` (for burn rate)
        - [ ] Populate `ForecastViewModel`
        - [ ] Return `View(model)`
    - [ ] Add `[HttpPost] GenerateForecast(ScenarioType type, int daysAhead)` action

### 7.3 Update Razor Views to Bind Real Data
- [ ] **7.3.1 Update Dashboard View**
    - [ ] Open `Views/Home/Index.cshtml`
    - [ ] Replace hardcoded values:
        - [ ] Line 18: Change `$1,240,500` to `@Model.CurrentBalance.ToString("C")`
        - [ ] Line 15: Change `+4.2%` to `@Model.BalanceChangePercent.ToString("P1")`
        - [ ] Line 29: Change `$85,200` to `@Model.NetCashFlow30Day.ToString("C")`
        - [ ] Line 40: Change `+12.5%` to `@Model.ForecastGrowth60Day.ToString("P1")`
        - [ ] Line 51: Change `3 Items` to `@Model.ActiveAlertCount Items`
    - [ ] Update chart section (lines 72-95):
        - [ ] Replace static SVG paths with dynamic data from `@Model.HistoricalChartData`
        - [ ] Use `@foreach` loop to render data points
    - [ ] Update recent alerts section (lines 149-174):
        - [ ] Replace hardcoded alerts with `@foreach(var alert in Model.RecentAlerts)`
        - [ ] Bind `alert.Title`, `alert.Message`, `alert.TimeAgo`

- [ ] **7.3.2 Update Alerts View**
    - [ ] Open `Views/Alerts/Index.cshtml`
    - [ ] Replace hardcoded alert list (lines 35-133) with:
        - [ ] `@foreach(var alert in Model.TodayAlerts)` section
        - [ ] `@foreach(var alert in Model.YesterdayAlerts)` section
        - [ ] Bind severity class dynamically based on `alert.Severity`
        - [ ] Bind `alert.Title`, `alert.Message`
        - [ ] Add form/button for dismiss action

- [ ] **7.3.3 Update Transactions View**
    - [ ] Open `Views/Transactions/Index.cshtml`
    - [ ] Replace hardcoded table rows (lines 49-132) with:
        - [ ] `@foreach(var txn in Model.Transactions)` loop
        - [ ] Bind `txn.Date`, `txn.Description`, `txn.Category`, `txn.Type`, `txn.FormattedAmount`
        - [ ] Update pagination (line 140) with `Model.TotalCount`, `Model.CurrentPage`

- [ ] **7.3.4 Update Forecast View**
    - [ ] Open `Views/Forecast/Index.cshtml`
    - [ ] Update chart data points (lines 99-121):
        - [ ] Replace static SVG with dynamic rendering from `@Model.ActiveScenario.DataPoints`
        - [ ] Use `@foreach` to generate path coordinates
    - [ ] Update metrics cards (lines 169-179):
        - [ ] Bind `@Model.EndCashBalance`, `@Model.BurnRatePerMonth`
        - [ ] Bind `@Model.GrowthRate` (line 228)

---

## Phase 8: Testing, Error Handling & Final Polish

### 8.1 Testing & Verification
- [ ] **8.1.1 Database Integrity**
    - [ ] Run application
    - [ ] Verify database file `cashflow.db` exists
    - [ ] Verify seed data loaded (check transaction count)
    - [ ] Inspect database schema (verify indexes created)

- [ ] **8.1.2 Service Layer Testing**
    - [ ] Test TransactionService:
        - [ ] Create a new transaction via service
        - [ ] Verify it appears in database
        - [ ] Get transaction summary for last 30 days
        - [ ] Verify calculations are correct
    - [ ] Test AlertService:
        - [ ] Call `GenerateSystemAlertsAsync()`
        - [ ] Verify alerts created based on rules
        - [ ] Mark alert as read, verify status updated
    - [ ] Test ForecastService:
        - [ ] Generate base case forecast
        - [ ] Verify data points created
        - [ ] Check projection calculations manually

- [ ] **8.1.3 End-to-End UI Testing**
    - [ ] Navigate to Dashboard (`/`)
        - [ ] Verify KPI cards show real data (not hardcoded values)
        - [ ] Verify chart renders (even if basic)
        - [ ] Verify recent alerts display
    - [ ] Navigate to Alerts (`/Alerts`)
        - [ ] Verify alert list populated
        - [ ] Click "Mark as Read", verify status changes
    - [ ] Navigate to Transactions (`/Transactions`)
        - [ ] Verify transaction table populated
        - [ ] Click "Add Transaction" (if form exists)
    - [ ] Navigate to Forecast (`/Forecast`)
        - [ ] Verify forecast chart displays
        - [ ] Verify metrics cards show real data

### 8.2 Error Handling
- [ ] **8.2.1 Service Layer Validation**
    - [ ] Add try-catch blocks in all service methods
    - [ ] Throw custom exceptions (ValidationException, NotFoundException)
    - [ ] Log errors using ILogger

- [ ] **8.2.2 Controller Error Handling**
    - [ ] Wrap service calls in try-catch
    - [ ] Display user-friendly error messages
    - [ ] Return appropriate HTTP status codes (400, 404, 500)

- [ ] **8.2.3 Null Safety**
    - [ ] Review all repository methods for null returns
    - [ ] Add null checks before mapping to DTOs
    - [ ] Use null-conditional operators (?.  ??)

### 8.3 Performance Optimization
- [ ] **8.3.1 Query Optimization**
    - [ ] Review EF Core queries for N+1 issues
    - [ ] Add `.AsNoTracking()` to all read-only queries
    - [ ] Verify indexes are being used (check query plans)

- [ ] **8.3.2 Forecasting Performance**
    - [ ] Measure forecast generation time (add logging)
    - [ ] If > 1 second, consider pre-computing and caching
    - [ ] Add configuration option to limit forecast horizon

### 8.4 Code Quality & Documentation
- [ ] **8.4.1 Code Review**
    - [ ] Ensure all public methods have XML comments
    - [ ] Verify consistent naming conventions
    - [ ] Remove unused using statements
    - [ ] Ensure all classes follow SOLID principles

- [ ] **8.4.2 Configuration Validation**
    - [ ] Verify all settings in appsettings.json are used
    - [ ] Add default values for optional settings
    - [ ] Document configuration options in README.md

- [ ] **8.4.3 Logging**
    - [ ] Add structured logging to critical operations
    - [ ] Log forecast generation start/end times
    - [ ] Log alert rule evaluations
    - [ ] Log database query performance

### 8.5 Final Verification
- [ ] **8.5.1 Build & Run**
    - [ ] Run `dotnet build` (verify no warnings)
    - [ ] Run `dotnet run`
    - [ ] Navigate to all pages, verify no runtime errors

- [ ] **8.5.2 Visual QA**
    - [ ] Complete Phase 3 visual verification tasks (mobile/desktop)
    - [ ] Verify all data bindings display correctly
    - [ ] Check for layout issues with real data

- [ ] **8.5.3 Prepare for Demo**
    - [ ] Create demo script (sequence of actions to show)
    - [ ] Prepare talking points for hackathon judges
    - [ ] Record demo video (required by hackathon)
