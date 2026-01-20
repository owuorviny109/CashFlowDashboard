# 3. Strict ViewModel Pattern

## Context
In ASP.NET Core MVC, data can be passed from Controller to View in multiple ways:
1. `ViewBag` / `ViewData` (Dynamic/Dictionary-based)
2. Domain Models (Direct Entity Framework classes)
3. Dedicated ViewModels (DTOs specific to the View)

Using dynamic objects (`ViewBag`) is fast to write but prone to runtime errors as property names are not checked at compile time.
Using Domain Models directly couples the external interface (View) to the internal database schema, potentially leading to over-posting vulnerabilities and rigid code structure.

## Decision
A **Strict ViewModel Pattern** is enforced.
- Every View must have a corresponding strongly-typed `ViewModel` class (e.g., `DashboardViewModel`, `TransactionsViewModel`).
- Controllers must map internal data to these ViewModels before rendering.
- `ViewBag` usage is strictly limited to transient UI messages (e.g., "Page Title" or simple validation flags) and not business data.

## Consequences

**Positive**:
- **Type Safety**: The compiler ensures that the View receives exactly the data it expects. Renaming a property causes an immediate build failure (Fail Fast), preventing runtime errors.
- **Security**: Prevents "Mass Assignment" or accidental exposure of sensitive internal data (e.g., Password hashes, Internal IDs) that might exist on Domain entities.
- **Maintainability**: The ViewModel acts as a clear contract or "Interface" for the UI.

**Negative**:
- **Verbosity**: Requires creating a separate class and mapping logic for every page, increasing file count.

**Mitigation**:
- The increased verbosity is accepted as a necessary trade-off for the stability and robustness required for a high-stakes hackathon demo, where a runtime "NullReferenceException" is unacceptable.
