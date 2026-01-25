# CashFlowDashboard 

[![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
[![Chart.js](https://img.shields.io/badge/Chart.js-FF6384?style=for-the-badge&logo=chartdotjs&logoColor=white)](https://www.chartjs.org/)
[![Hackathon](https://img.shields.io/badge/Hackathon-DVHacks%202026-F9AB00?style=for-the-badge)](https://dvhacks2026.devpost.com/)

> **Predictive Financial Intelligence for Small Business.**  
> *Because 82% of small businesses fail due to cash flow problems, not profit [1].*

---

## The Problem
Small business owners fly blind. They know what's in the bank *today*, but they have no visibility into *next month*.
*   **ERP Systems** are too expensive for micro-enterprises (often exceeding $2,000/mo for full suites [2]).
*   **Spreadsheets** are error-prone and static.
*   **Result:** Usable cash runs out unexpectedly, leading to failure.

## The Solution
**CashFlowDashboard** is an algorithmic financial command center designed to democratize Fortune 500-level forecasting for main street businesses.

It doesn't just track meaningful data; it **projects** it using statistical analysis to answer the #1 question: *"Will I have enough cash in 30 days?"*

## Key Features

### 1. Algorithmic Forecasting Engine
**Implements** strict **Linear Regression Analysis** to calculate the business's "Burn Rate Slope" and project cash positions 30, 60, and 90 days out.
*   **Base Case:** Pure statistical trend.
*   **Optimistic/Pessimistic:** Dynamic "What-If" scenarios configurable in settings.
*   **Confidence Intervals:** Visualizes volatility risk (Standard Deviation bounds).

### 2. Intelligent Alert System
The system acts as a 24/7 CFO that watches the data:
*   **Runway Alerts:** "Projected balance drops below $10,000 in 14 days." (Predictive)
*   **Anomaly Detection:** Flags unusually large transactions or spending spikes.
*   **Smart Filtering:** Prioritizes Critical issues vs Warnings.

### 3. Frictionless Experience
*   **Glassmorphism UI:** A beautiful, stunning interface built with Tailwind CSS.
*   **Instant Interaction:** AJAX-powered filtering and modal-based editing (no page reloads).
*   **Mobile First:** Fully responsive for valid-on-the-go analysis.

---

## Technical Architecture

Built on the **Principles of Clean Architecture** to ensure enterprise-grade resilience:

*   **Core:** ASP.NET Core 8 MVC (Server-Side Rendering for speed).
*   **Data Access:** Repository Pattern over Entity Framework Core (SQLite).
*   **Logic:** Dedicated Service Layer (`ForecastService`, `AlertService`) separating math from presentation.
*   **Frontend:** Tailwind CSS + Chart.js (Zero JavaScript framework bloat).

> **See the Architecture Decision Records (ADRs) for deep dives:**
> *   [ADR 0005: Service Layer & Forecasting Logic](Docs/adr/0005-service-layer-architecture.md)
> *   [ADR 0006: Resilience & Configuration](Docs/adr/0006-rearchitecture-and-resilience.md)

---

## Getting Started (Run it in 60 seconds)

For rapid evaluation and judging:

1.  **Clone & Enter**
    ```bash
    git clone https://github.com/owuorviny109/CashFlowDashboard.git
    cd CashFlowDashboard/CashFlowDashboard
    ```

2.  **Run with Seed Data (CRITICAL STEP)**
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
[1] U.S. Bank study cited by SCORE.org. "The #1 Reason Small Businesses Fail - Cash Flow".  
[2] "Average Cost of ERP Systems for Small Business" (Top10ERP.org, Softwhere.co). estimates range significantly, with comprehensive cloud suites often starting at $2,000/mo total cost of ownership.

---

## License
Distributed under the MIT License. See [LICENSE](LICENSE) for more information.

*"Built with .NET for DVHacks 2026"*
