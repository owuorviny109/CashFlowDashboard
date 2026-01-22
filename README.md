# CashFlowDashboard

[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
[![Hackathon](https://img.shields.io/badge/Hackathon-DVHacks%202026-F9AB00?style=for-the-badge)](https://dvhacks2026.devpost.com/)

> **Empowering small businesses with crystal-clear financial visibility.**

## Overview

**CashFlowDashboard** is being built for the **[DVHacks 2026](https://dvhacks2026.devpost.com/?ref_feature=challenge&ref_medium=discover)** hackathon.

**Theme:** *Innovating for Business Resilience.*
This event challenges developers to spend 2 weeks crafting solutions that solve real-world problems, with a specific focus on tools that help businesses survive and thrive in volatile markets.

## Problem Statement

Small business owners and freelancers often fly blind when it comes to cash flow.
- **Complexity:** Existing tools are bloated accounting suites that require a degree to navigate.
- **Cost:** Enterprise-grade forecasting is too expensive for early-stage ventures.
- **Uncertainty:** Without clear visibility into future cash positions, businesses struggle to make informed hiring or purchasing decisions, often leading to avoidable liquidity crunches.

## Solution

CashFlowDashboard is a lightweight, focused tool designed to bridge the gap between "back-of-the-napkin" math and expensive ERP systems. It provides:

- **Real-time Tracking:** Instantly record and view cash inflows and outflows.
- **Visual Trends:** Intuitive charts that make financial health understandable at a glance.
- **Smart Forecasting:** Projections for 30, 60, and 90 days to help owners peer into the future.
- **Actionable Alerts:** Specialized notifications for low balance thresholds or unusual spending patterns.

This solution directly addresses the hackathon's goal of *helping businesses* by giving owners the clarity they need to stay solvent and grow.

## Key Features

This project is currently in its **MVP (minimum viable product)** phase, developed specifically for the hackathon:

- ** Executive Dashboard:** A clean, responsive overview tracking current balance and recent activity.
- ** Transaction Management:** Easy-to-use interface to Add, Edit, and List income and expenses.
- ** Forecast Visualization:** A dedicated view showing projected cash availability.
- ** Alerts System:** A notification center for critical financial warnings.
- ** User Configuration:** Adjustable settings for forecast horizons, alert thresholds, and scenarios.
- ** Modern UI:** Built with **Tailwind CSS** for a professional, responsive mobile-first experience.
- **Backend:** Powered by **ASP.NET Core MVC** for robust server-side logic.

## Architecture Decisions (ADR)

This project maintains a record of significant architectural choices to ensure design consistency and transparency:
- [ADR 0001: Use Tailwind CSS via CDN](Docs/adr/0001-use-tailwind-cdn.md)
- [ADR 0002: Server-Side Rendering Strategy](Docs/adr/0002-server-side-rendering-strategy.md)
- [ADR 0003: Strict ViewModel Pattern](Docs/adr/0003-strict-viewmodel-pattern.md)
- [ADR 0004: Repository Pattern for Data Access](Docs/adr/0004-repository-pattern-for-data-access.md)
- [ADR 0005: Service Layer Architecture](Docs/adr/0005-service-layer-architecture.md)
- [ADR 0006: Rearchitecture, Configuration, and Resilience](Docs/adr/0006-rearchitecture-and-resilience.md)
- [Backend Architecture Specification](Docs/backend-architecture.md)

## Project Roadmap

For a detailed breakdown of the development progress and module implementation status, please refer to the [Project Roadmap](Docs/task.md).

## How to Run

To run this project locally, ensure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed.

1. **Clone the repository:**
   ```bash
   git clone https://github.com/owuorviny109/CashFlowDashboard.git
   ```

2. **Navigate to the project folder:**
   ```bash
   cd CashFlowDashboard/CashFlowDashboard
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

4. **Open in Browser:**
   The terminal will display the local URL (e.g., `http://localhost:5000`).

## Demo & Submission

- **Video Demo:** *[Link to Demo Video - Coming Soon]*
- **Devpost Submission:** *[Link to Devpost Page - Coming Soon]*

## Judging Criteria Alignment

- **Relevance:** Directly tackles the high failure rate of small businesses due to cash flow mismanagement.
- **Quality:** Clean architecture using ASP.NET Core best practices and a polished UI.
- **Usability:** Zero-learning-curve interface designed for busy non-technical owners.
- **Impact:** Democratizes financial intelligence for the smallest micro-enterprises.

## Future Enhancements
*Planned for post-hackathon development:*

- **Bank API Integration:** Automating transaction feeds via Plaid or Yodlee.
- **Export Capabilities:** Generating PDF/Excel reports for tax season.
- **Multi-User Support:** Allowing accountants and owners to collaborate.
- **Machine Learning:** More accurate forecasting based on seasonality and historical data.

## License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.
