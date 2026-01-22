# 5. Service Layer Architecture

## Context
After implementing the Repository Layer (ADR 0004), the application needs a dedicated layer to encapsulate complex business logic, algorithmic computations, and cross-cutting concerns. 

Directly placing business logic in Controllers has critical drawbacks:
1.  **Fat Controllers**: Controllers become bloated with validation, calculations, and orchestration logic, violating Single Responsibility Principle.
2.  **Duplication**: Business rules (e.g., "Low Balance Alert threshold") get duplicated across multiple endpoints.
3.  **Testability**: Testing complex algorithms (e.g., forecasting) becomes difficult when tightly coupled to HTTP request/response cycles.
4.  **Algorithmic Complexity**: The forecasting engine requires statistical computations (Linear Regression, Standard Deviation) that don't belong in the presentation layer.

## Decision
Implement a **Service Layer** to act as the core business logic orchestrator, sitting between Controllers and Repositories.

### Architecture Components

**1. Service Interfaces (`Services/Interfaces/`)**
- Define clear contracts for all business operations.
- Services are injected into Controllers via dependency injection.
- Examples: `ITransactionService`, `IAlertService`, `IForecastService`, `ICashFlowAnalyticsService`.

**2. Data Transfer Objects (`Services/DTOs/`)**
- Strict data contracts separate from Domain Entities.
- **Command DTOs**: Encapsulate user input with validation attributes (e.g., `CreateTransactionCommand`).
- **Query DTOs**: Shape data for presentation without exposing raw entities (e.g., `TransactionDto`, `ForecastScenarioDto`).
- Prevents over-posting attacks and keeps API surface clean.

**3. Forecasting Algorithm (ForecastService)**
- **Linear Regression**: Projects future daily cash flow by analyzing historical net flow trends (Slope + Intercept).
- **Standard Deviation Bounds**: Calculates ±2σ confidence intervals (95% confidence) using Random Walk theory.
- **Fallback Strategy**: If < 30 days of historical data, uses simple averaging instead of regression (prevents unstable predictions).
- **Scenario Modeling**: Supports Base Case (1.0x), Optimistic (1.2x), and Pessimistic (0.8x) multipliers.

**4. Alert Rule Engine (AlertService)**
- **Rule 1 - Low Balance**: Triggers when current balance < configurable threshold (default: 10,000).
- **Rule 2 - Large Transaction**: Flags transactions > threshold (default: 50,000) for review.
- **Rule 3 - Projected Shortfall**: Analyzes forecast scenarios to warn of future negative balances.
- **Deduplication Logic**: Uses `TriggeredBy` field to prevent duplicate alerts for the same event.

**5. Configuration-Driven Thresholds**
- `AlertSettings` and `ForecastSettings` classes bind to `appsettings.json`.
- Enables runtime tuning without code changes.
- Injected via `IOptions<T>` pattern.

## Consequences

**Positive**:
- **Separation of Concerns**: Business logic is fully decoupled from presentation and data access layers.
- **Testability**: Services can be unit tested in isolation by mocking repositories (no database or HTTP context required).
- **Reusability**: Forecasting and alert logic can be invoked from background jobs, APIs, or CLI tools without duplication.
- **Algorithmic Rigor**: Statistical methods are documented, version-controlled, and peer-reviewable.
- **Operational Control**: Thresholds can be adjusted per environment (dev/staging/prod) via configuration.

**Negative**:
- **Increased Complexity**: Adds another abstraction layer (Interfaces + Implementations).
- **Mapping Overhead**: Requires manual mapping between Entities and DTOs (no AutoMapper used to maintain explicit control).
- **Learning Curve**: Developers must understand the flow: Controller → Service → Repository → Database.

**Mitigation**:
- The cost of complexity is offset by gains in **testability**, **maintainability**, and **operational resilience** (Systems Engineering Doctrine).
- DTOs are kept simple (immutable `record` types) to minimize mapping boilerplate.
- All algorithmic decisions are documented with inline comments explaining "Why" over "What".
