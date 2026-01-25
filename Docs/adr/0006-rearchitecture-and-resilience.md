# 6. Rearchitecture, Configuration, and Resilience




## Context
The application encountered several critical impediments to stability and usability:

**1. Build Failures**  
The initial project structure placed `Services` and `Configuration` libraries outside the main web project, causing persistent namespace resolution and assembly reference errors during compilation. The build system could not properly resolve dependencies across separate project boundaries.

**2. Runtime Instability**  
The `Forecast` page crashed with a `NotSupportedException` when attempting to calculate balance metrics. The EF Core SQLite provider cannot translate `Sum(decimal)` aggregate operations to SQL, as SQLite's type affinity system treats decimals as either TEXT or REAL, which the provider cannot reliably map for server-side aggregation.

**3. Rigid Configuration**  
Alert thresholds, forecasting parameters, and business logic multipliers were locked in static `appsettings.json` files. Changing these values required code deployments, preventing non-technical users from tuning the system to their business context (e.g., adjusting low balance thresholds or forecast horizons).

**4. Lack of Observability**  
The application lacked structured logging and user-friendly error handling mechanisms. Runtime exceptions produced raw stack traces, debugging required manual console inspection, and there was no audit trail for production incidents.

## Decision

The following architectural and structural changes have been implemented:

**1. Consolidate Project Structure**  
The `Services` and `Configuration` directories have been moved inside the `CashFlowDashboard` web project root.
- *Rationale:* Eliminates complex project reference management for this monolithic MVP, ensuring a reliable build pipeline and simplifying namespace usage. For the current scope (single-deployment hackathon project), the benefits of separate assemblies do not justify the maintenance overhead.

**2. Database-Backed User Configuration**  
A dynamic configuration system has been introduced, backed by a new `AppSetting` entity and corresponding database table.
- *Implementation:* Key-value store in SQLite allows runtime updates via a new `SettingsController` and dedicated UI (`Views/Settings/Index.cshtml`).
- *Rationale:* Enables "hot reloading" of business logic parameters (Growth Multipliers, Alert Thresholds) without requiring code changes or application restarts. Non-technical users can now calibrate the system through the web interface.

**3. Client-Side Aggregation for SQLite**  
`TransactionRepository.GetBalanceAtDateAsync` has been modified to fetch only required fields (`Type`, `Amount`) and perform summation in-memory.
- *Rationale:* Bypasses the EF Core SQLite provider limitation regarding decimal aggregates. While this approach is O(N) and loads data into memory, the performance impact is negligible for the target dataset size (small business transactions, typically <10,000 records per year). For high-scale scenarios, this would be replaced with raw SQL or a different provider (PostgreSQL, SQL Server).

**4. Structured Logging & Global Error Handling**  
**Serilog** has been integrated for structured logging (Console + File sinks), and a global exception handling strategy has been implemented using `ErrorController`.
- *Implementation:* 
  - `UseExceptionHandler("/Error")` captures unhandled exceptions and routes to a custom error view.
  - `UseStatusCodePagesWithReExecute("/Error/{0}")` handles 404/403/500 status codes gracefully.
  - Logs are written to `logs/log-{Date}.txt` with automatic daily rotation.
- *Rationale:* Essential for production readiness (Systems Engineering Doctrine). Provides audit trails, enables post-mortem analysis, and ensures users see friendly error pages instead of raw stack traces.

## Consequences

**Positive:**
- **Build Stability**: The application compiles reliably without namespace resolution errors.
- **Runtime Reliability**: The Forecast page no longer crashes on balance calculations.
- **User Empowerment**: Business users can adjust forecasting and alerting behavior without developer intervention.
- **Observability**: Errors are logged to persistent files with structured context (timestamps, request IDs), enabling incident analysis.
- **User Experience**: Generic 404/500 errors are replaced with branded error pages that maintain the application's design system.

**Negative:**
- **Performance Risk**: Client-side aggregation loads more data into memory than server-side `Sum()`. This is acceptable for the current scope but would require optimization (raw SQL, stored procedures, or provider change) for datasets exceeding 100,000 transactions.
- **Coupling**: Moving services into the web project increases coupling, making it slightly harder to split into microservices later. However, this is not an immediate concern for the MVP phase.
- **Settings Seeding**: The `SettingsController` seeds default values on first access. If multiple requests hit this endpoint simultaneously before seeding completes, race conditions could occur (mitigated by EF Core transaction handling, but worth noting).

**Mitigation:**
- Performance monitoring has been added via Serilog request logging to detect if aggregation queries exceed acceptable thresholds.
- The Settings table is intentionally small (< 50 rows) and indexed by `Key` (primary key), ensuring fast lookups.
- Documentation has been added to `task.md` marking this aggregation strategy as a "known technical debt item" for post-hackathon optimization.

## References
- [EF Core SQLite Limitations](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations)
- [Serilog Best Practices](https://github.com/serilog/serilog/wiki/Best-Practices)
- [ASP.NET Core Error Handling](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
