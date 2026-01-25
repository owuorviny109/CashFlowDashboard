# CashFlowDashboard 

[![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
[![Chart.js](https://img.shields.io/badge/Chart.js-FF6384?style=for-the-badge&logo=chartdotjs&logoColor=white)](https://www.chartjs.org/)
[![Hackathon](https://img.shields.io/badge/Hackathon-DVHacks%202026-F9AB00?style=for-the-badge)](https://dvhacks2026.devpost.com/)

**Submission for [DVHacks 2026](https://dvhacks2026.devpost.com/)**

> **Predictive Financial Intelligence for Small Business.**  
> *Because 82% of small businesses fail due to cash flow problems, not profit [1].*

---

## The Problem
Small business owners often fly blind. They know what's in the bank *today*, but they have no visibility into *next month*.
*   **Cash Flow Stress:** Seasonal swings and late payments make cash flow a top stress point, jeopardizing survival [1].
*   **Financial Uncertainty:** Businesses often don't know when shortfalls will hit until it's too late, leading to poor planning decisions [2].
*   **Result:** Usable cash runs out unexpectedly, leading to failure.

## App Technology Stack

The application is built using:
*   **ASP.NET Core MVC** – The modern web framework for C# apps. It combines controllers, views, and models in a clean architecture and runs cross-platform.
*   **.NET 8 (LTS)** – The recommended long-term support version for stability and future extensibility (cloud, hosting, features).
*   **Entity Framework Core** – For database access and automated schema migrations.
*   **Database** – SQLite (for simplicity) or PostgreSQL (production ready).
*   **Charting UI** – **Chart.js** integrated for visual cash flow reports.

## App Purpose & Features

The application performs these core functions:

### Dashboard
*   **Current Cash Position**: Shows total cash available now (`DashboardViewModel`).
*   **Cash Flow Trends**: Visualizes changes over time using `CashFlowAnalyticsService`.
*   **Visual Charts**: Helps owners see financial health at a glance.

### Forecasting & Alerts
*   **Future Projections**: Projects cash positions 30-90 days out (`ForecastService`).
*   **Smart Alerts**: Notifications for low balances via `AlertsController`.
*   **Scenario Planning**: Switch between Base, Optimistic, and Pessimistic scenarios.

### Simple Data Entry
*   **Transaction Management**: Forms to enter incomes received and expenses paid (`TransactionsController`).
*   **Recurring Items**: Architecture supports recurring transaction definitions.

### Reports & Exports
*   **Downloadable Reports**: Generate PDF/CSV reports for income/expenses and projections.

## Project Architecture (High-Level)

The code follows a strict **Clean MVC** structure:

### Models
Represent your data:
*   **Transaction** – Holds amount, type (income/expense), date, description.
*   **CashFlowForecast** – Calculated data for predictions.

### Views
Use Razor views to render UI:
*   **Dashboard** – Main visual page with charts and summaries.
*   **Transactions** – Add/edit list of incomes/expenses with modal interactions.
*   **Forecast** – Show predicted cash flow with confidence intervals.

### Controllers
Handle requests:
*   **DashboardController** – Loads and formats data for visuals.
*   **TransactionsController** – CRUD operations for incomes/expenses.
*   **AlertsController** – Generates and filters system alerts.

### Data Access Layer
*   **Repository Pattern**: Uses Entity Framework Core to manage migrations and queries, abstracting the `DbContext`.

### Architecture Decision Records
For detailed technical decisions and trade-off analysis:
*   [Backend Architecture Specification](Docs/backend-architecture.md)
*   [ADR 0001: Use Tailwind CSS via CDN](Docs/adr/0001-use-tailwind-cdn.md)
*   [ADR 0002: Server-Side Rendering Strategy](Docs/adr/0002-server-side-rendering-strategy.md)
*   [ADR 0003: Strict ViewModel Pattern](Docs/adr/0003-strict-viewmodel-pattern.md)
*   [ADR 0004: Repository Pattern for Data Access](Docs/adr/0004-repository-pattern-for-data-access.md)
*   [ADR 0005: Service Layer Architecture](Docs/adr/0005-service-layer-architecture.md)
*   [ADR 0006: Rearchitecture and Resilience](Docs/adr/0006-rearchitecture-and-resilience.md)

---

## Getting Started (Run it in 60 seconds)

For rapid evaluation and judging:

1.  **Clone & Enter**
    ```bash
    git clone https://github.com/owuorviny109/CashFlowDashboard.git
    cd CashFlowDashboard/CashFlowDashboard
    ```

2.  **Run with Seed Data**
    *Running with `seed-history` instantly populates 6 months of realistic transaction data, ensuring the Forecast Engine functionality is immediately visible.*
    ```bash
    dotnet run -- seed-history
    ```

3.  **Launch**
    Open `http://localhost:5236` (or the port shown in the terminal).

---

## Future Roadmap
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
