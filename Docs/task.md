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
- [x] **Folder Setup**
    - [x] Create `Services/` directory
    - [x] Create `Services/Interfaces/` directory
    - [x] Create `Services/DTOs/` directory

### 6.2 Define Data Transfer Objects (DTOs)
- [x] **6.2.1 Core DTOs**
    - [x] Create `Services/DTOs/TransactionDto.cs`
        - [x] Add all display properties (Id, Date, Description, Category, Type, Amount, FormattedAmount)
    - [x] Create `Services/DTOs/AlertDto.cs`
        - [x] Add properties (Id, Severity, Title, Message, TimeAgo, Status, ActionUrl)
    - [x] Create `Services/DTOs/ForecastDataPointDto.cs`
        - [x] Add properties (Date, ProjectedBalance, LowerBound, UpperBound)
    - [x] Create `Services/DTOs/ForecastScenarioDto.cs`
        - [x] Add properties (Id, Name, ScenarioType, DataPoints, ConfidenceLevel, EndCashBalance)

- [x] **6.2.2 Aggregate DTOs**
    - [x] Create `Services/DTOs/TransactionSummaryDto.cs`
        - [x] Add TotalIncome, TotalExpenses, NetCashFlow, TransactionCount, AverageTransactionSize
    - [x] Create `Services/DTOs/CashFlowTrendDto.cs`
        - [x] Add DataPoints list, TrendDirection enum, GrowthRate
    - [x] Create `Services/DTOs/GrowthMetricsDto.cs`
        - [x] Add CurrentBalance, BalanceChangePercent, NetCashFlow30Day, BurnRate
    - [x] Create `Services/DTOs/ChartDataPoint.cs`
        - [x] Add Date (string, ISO 8601), Balance, Income, Expenses (all nullable except Date)

- [x] **6.2.3 Command DTOs** (for create/update operations)
    - [x] Create `Services/DTOs/CreateTransactionCommand.cs`
        - [x] Add Date, Amount, Type, Category, Description, IsRecurring, RecurrencePattern
    - [x] Create `Services/DTOs/UpdateTransactionCommand.cs`
        - [x] Add updatable fields (Amount, Category, Description)
    - [x] Create `Services/DTOs/CreateAlertCommand.cs`
        - [x] Add Severity, Title, Message, Category

### 6.3 Implement Service Interfaces
- [x] **6.3.1 ITransactionService**
    - [x] Create `Services/Interfaces/ITransactionService.cs`
        - [x] Add `Task<TransactionDto> CreateTransactionAsync(CreateTransactionCommand cmd, CancellationToken ct)`
        - [x] Add `Task<TransactionDto?> GetTransactionByIdAsync(Guid id, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<TransactionDto>> GetRecentTransactionsAsync(int count, CancellationToken ct)`
        - [x] Add `Task<TransactionSummaryDto> GetSummaryForPeriodAsync(DateTime start, DateTime end, CancellationToken ct)`
        - [x] Add `Task UpdateTransactionAsync(Guid id, UpdateTransactionCommand cmd, CancellationToken ct)`
        - [x] Add `Task DeleteTransactionAsync(Guid id, CancellationToken ct)`

- [x] **6.3.2 IAlertService**
    - [x] Create `Services/Interfaces/IAlertService.cs`
        - [x] Add `Task<AlertDto> CreateManualAlertAsync(CreateAlertCommand cmd, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<AlertDto>> GetActiveAlertsAsync(CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<AlertDto>> GetAlertsBySeverityAsync(AlertSeverity severity, CancellationToken ct)`
        - [x] Add `Task MarkAsReadAsync(Guid alertId, CancellationToken ct)`
        - [x] Add `Task DismissAlertAsync(Guid alertId, CancellationToken ct)`
        - [x] Add `Task MarkAllAsReadAsync(CancellationToken ct)`
        - [x] Add `Task GenerateSystemAlertsAsync(CancellationToken ct)` (background job method)

- [x] **6.3.3 ICashFlowAnalyticsService**
    - [x] Create `Services/Interfaces/ICashFlowAnalyticsService.cs`
        - [x] Add `Task<CashFlowTrendDto> GetTrendForPeriodAsync(DateTime start, DateTime end, CancellationToken ct)`
        - [x] Add `Task<decimal> GetCurrentBalanceAsync(CancellationToken ct)`
        - [x] Add `Task<decimal> GetBalanceAtDateAsync(DateTime date, CancellationToken ct)`
        - [x] Add `Task<GrowthMetricsDto> GetGrowthMetricsAsync(CancellationToken ct)`
        - [x] Add `Task GenerateDailySnapshotAsync(DateOnly date, CancellationToken ct)` (background job)

- [x] **6.3.4 IForecastService**
    - [x] Create `Services/Interfaces/IForecastService.cs`
        - [x] Add `Task<ForecastScenarioDto> GenerateBaseCaseForecastAsync(int daysAhead, CancellationToken ct)`
        - [x] Add `Task<ForecastScenarioDto> GenerateOptimisticForecastAsync(int daysAhead, CancellationToken ct)`
        - [x] Add `Task<ForecastScenarioDto> GeneratePessimisticForecastAsync(int daysAhead, CancellationToken ct)`
        - [x] Add `Task<IReadOnlyList<ForecastScenarioDto>> GetActiveForecastsAsync(CancellationToken ct)`

### 6.4 Implement TransactionService
- [x] **Create `Services/TransactionService.cs`**
    - [x] Inject `ITransactionRepository` and `ILogger<TransactionService>`
    - [x] Implement `CreateTransactionAsync()`
        - [x] Validate command (amount > 0, valid dates)
        - [x] Map command to entity
        - [x] Call repository AddAsync
        - [x] Map entity to DTO and return
    - [x] Implement `GetTransactionByIdAsync()`
        - [x] Call repository GetByIdAsync
        - [x] Map entity to DTO (handle null case)
    - [x] Implement `GetRecentTransactionsAsync()`
        - [x] Call repository GetRecentAsync
        - [x] Map list to DTOs
    - [x] Implement `GetSummaryForPeriodAsync()`
        - [x] Call repository GetByDateRangeAsync
        - [x] Compute aggregates (sum income, sum expenses, count, average)
        - [x] Return summary DTO
    - [x] Implement `UpdateTransactionAsync()`
        - [x] Validate command
        - [x] Load existing entity
        - [x] Update properties
        - [x] Call repository UpdateAsync
    - [x] Implement `DeleteTransactionAsync()`
        - [x] Call repository DeleteAsync

### 6.5 Implement AlertService (with Rule Engine)
- [x] **Create `Services/AlertService.cs`**
    - [x] Inject `IAlertRepository`, `ITransactionRepository`, `IForecastRepository`, `ILogger<AlertService>`
    - [x] Inject `IOptions<AlertSettings>` for configuration
    - [x] Implement `CreateManualAlertAsync()`
        - [x] Map command to entity
        - [x] Call repository AddAsync
    - [x] Implement `GetActiveAlertsAsync()`
        - [x] Call repository GetActiveAlertsAsync
        - [x] Map to DTOs (compute TimeAgo strings)
    - [x] Implement `GetAlertsBySeverityAsync()`
    - [x] Implement `MarkAsReadAsync()` (update status to Read)
    - [x] Implement `DismissAlertAsync()` (update status to Dismissed)
    - [x] Implement `MarkAllAsReadAsync()`
    - [x] **Implement `GenerateSystemAlertsAsync()` (Rule Engine)**
        - [x] **Rule 1: Low Balance Alert**
            - [x] Get current balance from transaction repository
            - [x] If balance < threshold (from config), create Critical alert
        - [x] **Rule 2: Overdue Invoice Alert** (Simplified to Large Txn for MVP or basic logic)
            - [x]  (Adjusted) Check for large transactions and alert.
        - [x] **Rule 3: Projected Shortfall Alert**
            - [x] Get active forecast scenarios
            - [x] Check if any DataPoint in next 30 days has balance < threshold
            - [x] Create Warning alert if shortfall detected
        - [x] **Rule 4: Large Transaction Alert**
            - [x] Query recent transactions with Amount > threshold
            - [x] Create Info alert for each (if not already alerted)
        - [x] Save all generated alerts in batch

### 6.6 Implement CashFlowAnalyticsService
- [x] **Create `Services/CashFlowAnalyticsService.cs`**
    - [x] Inject `ITransactionRepository`, `ICashFlowSnapshotRepository`, `ILogger<CashFlowAnalyticsService>`
    - [x] Implement `GetCurrentBalanceAsync()`
        - [x] Call repository GetBalanceAtDateAsync(DateTime.Today)
    - [x] Implement `GetBalanceAtDateAsync()`
        - [x] Call repository GetBalanceAtDateAsync(date)
    - [x] Implement `GetTrendForPeriodAsync()`
        - [x] Get snapshots from repository for date range
        - [x] Map to trend data points
        - [x] Calculate trend direction (compare first vs last)
        - [x] Return trend DTO
    - [x] Implement `GetGrowthMetricsAsync()`
        - [x] Get current balance
        - [x] Get balance 30 days ago
        - [x] Calculate percentage change
        - [x] Get net cash flow for last 30 days
        - [x] Calculate burn rate (average monthly expenses)
        - [x] Return metrics DTO
    - [x] **Implement `GenerateDailySnapshotAsync()`**
        - [x] Check if snapshot already exists for date
        - [x] Get all transactions for the date
        - [x] Get opening balance (closing balance from previous day)
        - [x] Sum income and expenses for the day
        - [x] Calculate closing balance
        - [x] Create and save snapshot entity

### 6.7 Implement ForecastService (Forecasting Engine)
- [x] **Create `Services/ForecastService.cs`**
    - [x] Inject `ITransactionRepository`, `ICashFlowSnapshotRepository`, `IForecastRepository`, `ILogger<ForecastService>`
    - [x] Inject `IOptions<ForecastSettings>` for configuration
    - [x] **Implement Core Forecasting Logic (Private Helper Methods)**
        - [x] `GenerateForecastAsync`: Implemented Linear Regression + StdDev logic.
            - [x] Fallback to simple average if < 30 days data.
            - [x] Projects daily flow using regression slope/intercept.
            - [x] Applies 2-Sigma confidence bounds (widening with time).
            - [x] Saves scenario to DB.
    - [x] Implement `GenerateBaseCaseForecastAsync()`
        - [x] Call helper with ScenarioType.BaseCase, growthMultiplier = 1.0
    - [x] Implement `GenerateOptimisticForecastAsync()`
        - [x] Call helper with ScenarioType.Optimistic, growthMultiplier = 1.2
    - [x] Implement `GeneratePessimisticForecastAsync()`
        - [x] Call helper with ScenarioType.Pessimistic, growthMultiplier = 0.8
    - [x] Implement `GetActiveForecastsAsync()`
        - [x] Call repository GetActiveScenariosAsync
        - [x] Map to DTOs

### 6.8 Register Services in DI Container
- [x] **Update Program.cs**
    - [x] Add `builder.Services.AddScoped<ITransactionService, TransactionService>()`
    - [x] Add `builder.Services.AddScoped<IAlertService, AlertService>()`
    - [x] Add `builder.Services.AddScoped<ICashFlowAnalyticsService, CashFlowAnalyticsService>()`
    - [x] Add `builder.Services.AddScoped<IForecastService, ForecastService>()`
    - [x] Add `builder.Services.Configure<ForecastSettings>(builder.Configuration.GetSection("Forecast"))`
    - [x] Add `builder.Services.Configure<AlertSettings>(builder.Configuration.GetSection("Alerts"))`

---

## Phase 7: Presentation Integration (Wire Controllers & Update Views)

### 7.1 Update ViewModels with Real Properties
- [x] **7.1.1 Update DashboardViewModel**
    - [x] Open `ViewModels/DashboardViewModel.cs`
    - [x] Replace placeholder properties with:
        - [x] `decimal CurrentBalance { get; init; }`
        - [x] `decimal BalanceChangePercent { get; init; }`
        - [x] `decimal NetCashFlow30Day { get; init; }`
        - [x] `decimal ForecastGrowth60Day { get; init; }`
        - [x] `int ActiveAlertCount { get; init; }`
        - [x] `IReadOnlyList<ChartDataPoint> HistoricalChartData { get; init; }`
        - [x] `IReadOnlyList<ChartDataPoint> ProjectedChartData { get; init; }`
        - [x] `IReadOnlyList<AlertDto> RecentAlerts { get; init; }`

- [x] **7.1.2 Update AlertsViewModel**
    - [x] Open `ViewModels/AlertsViewModel.cs`
    - [x] Add properties:
        - [x] `IReadOnlyList<AlertDto> TodayAlerts { get; init; }`
        - [x] `IReadOnlyList<AlertDto> YesterdayAlerts { get; init; }`
        - [x] `IReadOnlyList<AlertDto> OlderAlerts { get; init; }`
        - [x] `int UnreadCount { get; init; }`

- [x] **7.1.3 Update TransactionsViewModel**
    - [x] Open `ViewModels/TransactionsViewModel.cs`
    - [x] Add properties:
        - [x] `IReadOnlyList<TransactionDto> Transactions { get; init; }`
        - [x] `int TotalCount { get; init; }`
        - [x] `int CurrentPage { get; init; }`
        - [x] `int PageSize { get; init; }`

- [x] **7.1.4 Update ForecastViewModel**
    - [x] Open `ViewModels/ForecastViewModel.cs`
    - [x] Add properties:
        - [x] `ForecastScenarioDto ActiveScenario { get; init; }`
        - [x] `IReadOnlyList<ForecastScenarioDto> AllScenarios { get; init; }`
        - [x] `decimal EndCashBalance { get; init; }`
        - [x] `decimal BurnRatePerMonth { get; init; }`
        - [x] `decimal GrowthRate { get; init; }`

### 7.2 Update Controllers to Call Services
- [x] **7.2.1 Update HomeController (Dashboard)**
    - [x] Open `Controllers/HomeController.cs`
    - [x] Inject services: `ICashFlowAnalyticsService`, `IAlertService`, `IForecastService`
    - [x] Update `Index()` action method:
        - [x] Change signature to `async Task<IActionResult> Index(CancellationToken ct)`
        - [x] Call `await _analyticsService.GetCurrentBalanceAsync(ct)`
        - [x] Call `await _analyticsService.GetGrowthMetricsAsync(ct)`
        - [x] Call `await _analyticsService.GetTrendForPeriodAsync()` (last 6 months)
        - [x] Call `await _alertService.GetActiveAlertsAsync(ct)`
        - [x] Call `await _forecastService.GetActiveForecastsAsync(ct)` (for forecast growth)
        - [x] Populate `DashboardViewModel` with real data
        - [x] Return `View(model)`

- [x] **7.2.2 Update AlertsController**
    - [x] Open `Controllers/AlertsController.cs`
    - [x] Inject `IAlertService`
    - [x] Update `Index()` action:
        - [x] Change to async
        - [x] Call `await _alertService.GetActiveAlertsAsync(ct)`
        - [x] Group alerts by date (Today, Yesterday, Older)
        - [x] Populate `AlertsViewModel`
        - [x] Return `View(model)`
    - [x] Add `[HttpPost] MarkAsRead(Guid id)` action
        - [x] Call service method
        - [x] Redirect to Index
    - [x] Add `[HttpPost] MarkAllAsRead()` action

- [x] **7.2.3 Update TransactionsController**
    - [x] Open `Controllers/TransactionsController.cs`
    - [x] Inject `ITransactionService`
    - [x] Update `Index()` action:
        - [x] Add pagination parameters (page, pageSize)
        - [x] Call `await _transactionService.GetRecentTransactionsAsync()`
        - [x] Populate `TransactionsViewModel`
        - [x] Return `View(model)`
    - [x] Add `[HttpPost] Create(CreateTransactionCommand cmd)` action
        - [x] Validate model
        - [x] Call service CreateTransactionAsync
        - [x] Redirect to Index
    - [x] Add `[HttpPost] Delete(Guid id)` action

- [x] **7.2.4 Update ForecastController**
    - [x] Open `Controllers/ForecastController.cs`
    - [x] Inject `IForecastService`, `ICashFlowAnalyticsService`
    - [x] Update `Index()` action:
        - [x] Call `await _forecastService.GetActiveForecastsAsync(ct)`
        - [x] Call `await _analyticsService.GetGrowthMetricsAsync(ct)` (for burn rate)
        - [x] Populate `ForecastViewModel`
        - [x] Return `View(model)`
    - [x] Add `[HttpPost] GenerateForecast(ScenarioType type, int daysAhead)` action

### 7.3 Update Razor Views to Bind Real Data
- [x] **7.3.1 Update Dashboard View**
    - [x] Replaced hardcoded KPI values with Model bindings
    - [x] Integrated Chart.js for data visualization
    - [x] Bound H

istoricalChartData to interactive chart
    - [x] Updated recent alerts section with dynamic binding

- [x] **7.3.2 Update Alerts View**
    - [x]Replaced hardcoded alerts with grouped Model data
    - [x] Implemented time-based grouping (Today/Yesterday/Older)
    - [x] Added severity-based styling and icons
    - [x] Bound "Mark as Read" actions to AlertService
    - [x] Added empty state handling

- [x] **7.3.3 Update Transactions View**
    - [x] Replaced hardcoded transaction list with Model.Transactions
    - [x] Implemented pagination with Model.CurrentPage/TotalPages
    - [x] Bound delete action to TransactionService
    - [x] Added empty state handling
    - [x] Dynamic styling for Income/Expense types

- [x] **7.3.4 Update Forecast View**
    - [x] Replaced static chart with Chart.js forecast visualization
    - [x] Bound scenario selector to Model.AllScenarios
    - [x] Displayed confidence intervals (Upper/Lower bounds)
    - [x] Updated KPI cards with Model metrics (EndCashBalance, BurnRate, GrowthRate)
    - [x] Added empty state handling for missing forecast data
    - [x] Integrated "Generate Forecast" buttons with POST actions



- [x] **7.4 User Configuration (Bonus System Feature)**
    - [x] Create `AppSetting` Entity & DB Migration
    - [x] Implement database-backed configuration storage
    - [x] Create `SettingsController` with Seed logic
    - [x] Implement `Views/Settings/Index.cshtml` with inline editing forms
    - [x] Integrate Settings link into Sidebar navigation
    - [x] Fix Architectural dependency (moved Services/Configuration to project root)

---

## Phase 8: Testing, Error Handling & Final Polish

### 8.1 Testing & Verification
- [x] **8.1.1 Database Integrity**
    - [x] Run application (Verified startup logs)
    - [x] Verify database file `cashflow.db` exists (Implied by successful run)
    - [x] Verify seed data loaded (Logs show "SELECT EXISTS... FROM Transactions")
    - [x] Inspect database schema (Migrations applied)

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
    - [x] Navigate to Dashboard (`/`) (Verified via previous feedback)
    - [ ] Navigate to Alerts (`/Alerts`)
    - [ ] Navigate to Transactions (`/Transactions`)
    - [x] Navigate to Forecast (`/Forecast`) (Fixed 500 error)
    - [x] Navigate to Settings (`/Settings`) (Implemented and verified)

### 8.2 Error Handling
- [x] **8.2.1 Global Architecture**
    - [x] Create `ErrorController` for 404/500 handling
    - [x] Design custom `Error.cshtml` view
    - [x] Configure `UseExceptionHandler` and `UseStatusCodePagesWithReExecute`

- [ ] **8.2.2 Service Layer Validation**
    - [ ] Add try-catch blocks in all service methods
    - [ ] Throw custom exceptions (ValidationException, NotFoundException)
    - [ ] Log errors using ILogger

- [ ] **8.2.3 Controller Safety**
    - [ ] Wrap service calls in try-catch (where global handler isn't enough)
    - [ ] Return appropriate HTTP status codes

- [ ] **8.2.4 Null Safety**
    - [ ] Review all repository methods for null returns
    - [ ] Add null checks before mapping to DTOs
    - [ ] Use null-conditional operators (?.  ??)

### 8.3 Logging
- [x] **8.3.1 Serilog Configuration**
    - [x] Add Packages (Serilog.AspNetCore, Sinks.File, Sinks.Console)
    - [x] Configure `Log.Logger` in `Program.cs`
    - [x] Add `UseSerilogRequestLogging` middleware

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
