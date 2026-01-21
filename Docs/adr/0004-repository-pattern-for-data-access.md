# 4. Repository Pattern for Data Access

## Context

When building the backend for CashFlowDashboard, a strategy is required for accessing and manipulating data in the SQLite database.
The default approach in many ASP.NET Core tutorials is to inject the Entity Framework Core `DbContext` directly into Controllers or Services.

While simple to start, direct `DbContext` usage has significant downsides:
1.  **Tight Coupling**: The Service layer becomes tightly coupled to Entity Framework Core, deeply nesting database concerns with business logic.
In building the backend for CashFlowDashboard, we need a strategy for accessing and manipulating data in the SQLite database.
The default approach in many ASP.NET Core tutorials is to inject the Entity Framework Core `DbContext` directly into Controllers or Services.

While simple to start, direct `DbContext` usage has significant downsides:
1.  **Tight Coupling**: The Service layer becomes tightly coupled to Entity Framework Core. deeply nesting database concerns with business logic.
2.  **Testability**: Mocking a `DbContext` for unit testing is notoriously difficult, error-prone, and discouraged by Microsoft.
3.  **Duplication**: Common queries (e.g., "Get Active Alerts") often get copy-pasted across multiple controllers, leading to maintenance headaches.

## Decision
 
The **Repository Pattern** is implemented to abstract data access.

- **Interfaces**: Specific interfaces are defined for each aggregate root (e.g., `ITransactionRepository`, `IAlertRepository`). These interfaces define contracts in terms of Domain Entities, not EF Core concepts.
- **Implementation**: Concrete implementations (e.g., `TransactionRepository`) wrap the `CashFlowDbContext`.
- **Dependency Injection**: Repositories are registered as Scoped services and injected into higher-level layers (Services) instead of the `DbContext`.
=======
We will implement the **Repository Pattern** to abstract data access.

- **Interfaces**: We will define specific interfaces for each aggregate root (e.g., `ITransactionRepository`, `IAlertRepository`). These interfaces will define contracts in terms of Domain Entities, not EF Core concepts.
- **Implementation**: We will create concrete implementations (e.g., `TransactionRepository`) that wrap the `CashFlowDbContext`.
- **Dependency Injection**: Repositories will be registered as Scoped services and injected into higher-level layers (Services) instead of the `DbContext`.

## Consequences

**Positive**:
- **Testability**: `ITransactionRepository` can be easily mocked to return sample data when unit testing the Service layer, without spinning up an in-memory database.
=======
- **Testability**: We can easily mock an `ITransactionRepository` to return sample data when unit testing the Service layer, without spinning up an in-memory database.
 
- **Separation of Concerns**: Business logic (Services) is separated from data access plumbing (EF Core).
- **Consistency**: Centralizes query logic (e.g., always filtering out deleted items or always including related data) in one place.

**Negative**:
- **Boilerplate**: Requires creating an Interface and a Class for every entity, increasing the initial development volume.
 
- **Abstraction Leaks**: Some complex EF Core features (like arbitrary IQueryable composition) are hidden, forcing the definition of explicit methods for every query variant.

**Mitigation**:
- The benefits of "Testability" and "Resilience" (Systems Thinking Doctrine) outweigh the cost of writing extra boilerplate. `IQueryable` is strictly not returned from repositories to ensure the abstraction remains leak-proof.
 
- **Abstraction Leaks**: Some complex EF Core features (like arbitrary IQueryable composition) are hidden, forcing us to define explicit methods for every query variant.

**Mitigation**:
- The benefits of "Testability" and "Resilience" (Systems Thinking Doctrine) outweigh the cost of writing extra boilerplate. We will also strictly avoid returning `IQueryable` from repositories to ensure the abstraction remains leak-proof.
 
