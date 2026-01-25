# CashFlowDashboard 

[![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
[![Chart.js](https://img.shields.io/badge/Chart.js-FF6384?style=for-the-badge&logo=chartdotjs&logoColor=white)](https://www.chartjs.org/)
[![Hackathon](https://img.shields.io/badge/Hackathon-DVHacks%202026-F9AB00?style=for-the-badge)](https://dvhacks2026.devpost.com/)

**Submission for [DVHacks 2026](https://dvhacks2026.devpost.com/)**

> **Predictive Financial Intelligence for Small Business.**  
> *Because 82% of small businesses fail due to cash flow problems, not profit [1].*

> 📋 **[📄 View Full Submission Presentation →](PRESENTATION.md)**  
> *Complete hackathon submission document with problem statement, solution, technical details, and all rubric requirements.*

---

## Overview

CashFlowDashboard is a predictive financial intelligence platform for small businesses. It uses **Linear Regression Analysis** to forecast cash positions 30-90 days ahead, providing early warnings for cash flow shortfalls.

**Key Features:**
*   Real-time cash flow dashboard with interactive charts
*   Multi-scenario forecasting (Base, Optimistic, Pessimistic)
*   Smart alert system for low balances and anomalies
*   Transaction management with search and filtering

## Technology Stack

*   **ASP.NET Core MVC 8** – Server-side rendering
*   **.NET 8 (LTS)** – Long-term support
*   **Entity Framework Core** – ORM with migrations
*   **SQLite** – Zero-config database (PostgreSQL-ready)
*   **Chart.js** – Financial visualizations
*   **Tailwind CSS** – Modern UI framework
*   **Serilog** – Structured logging

## Architecture

**Layered Architecture:**
```
Controllers (Presentation) → Services (Business Logic) → Repositories (Data Access) → Database
```

**Design Patterns:**
*   Repository Pattern for data abstraction
*   Service Layer for business logic encapsulation
*   DTO Pattern for data contracts
*   ViewModel Pattern for type-safe views

**Core Algorithms:**
*   **Linear Regression**: `ForecastService.cs` calculates slope/intercept for projections
*   **Alert Rules**: `AlertService.cs` monitors thresholds and anomalies
*   **Confidence Intervals**: ±2σ (95% confidence) bounds for forecast uncertainty

For detailed architecture documentation, see [Backend Architecture Spec](Docs/backend-architecture.md).

## Documentation

*   **[PRESENTATION.md](PRESENTATION.md)** – Complete hackathon submission presentation
*   **[Backend Architecture](Docs/backend-architecture.md)** – Technical specification (850+ lines)
*   **[Architecture Decision Records](Docs/adr/)** – 6 ADRs documenting technical choices
*   **[Video Demo Script](Docs/VIDEO_SCRIPT.md)** – Step-by-step demo guide

---

## Getting Started (Run it in 60 seconds)

For rapid evaluation and judging:

1.  **Clone & Enter**
    ```bash
    git clone https://github.com/owuorviny109/CashFlowDashboard.git
    cd CashFlowDashboard/CashFlowDashboard
    ```

2.  **Run the Application**
    *The database will be created and seeded automatically with 6 months of realistic transaction data on first run.*
    ```bash
    dotnet run
    ```

3.  **Launch**
    Open `http://localhost:5236` (or the port shown in the terminal).

---

## Future Roadmap
*   **Reports & Exports:** Generate PDF/CSV reports for income/expenses and projections.
*   **Bank Feeds:** Plaid integration for automated syncing.
*   **Tax Vault:** Auto-calculate estimated quarterly tax payments.
*   **Multi-Tenancy:** SaaS support for Accountants managing multiple clients.

For a detailed task breakdown, see the [Project Roadmap](Docs/task.md).

---

## References
[1] "Cash Flow Management — a Top Stress Point". [QuickBooks](https://quickbooks.intuit.com/r/cash-flow/cash-flow-problems/)
[2] "Small Business Pain Points: Financial Uncertainty". [PainOnSocial](https://painonsocial.com/blog/small-business-pain-points)

---

## License
Distributed under the MIT License. See [LICENSE](LICENSE) for more information.

*"Built with .NET for DVHacks 2026"*
