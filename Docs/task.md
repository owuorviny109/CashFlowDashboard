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

## Phase 3: Quality Assurance
- [ ] **Navigation Verification**
    - [ ] Test Sidebar "Dashboard" link -> `/`
    - [ ] Test Sidebar "Transactions" link -> `/Transactions`
    - [ ] Test Sidebar "Forecast" link -> `/Forecast`
    - [ ] Test Sidebar "System Alerts" link -> `/Alerts`
- [ ] **Visual Verification**
    - [ ] Check Mobile View (Hamburger menu, stacking)
    - [ ] Check Desktop View (Sidebar visibility)
