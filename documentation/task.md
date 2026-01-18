# CashFlowDashboard: Granular Development Roadmap

## Phase 1: Foundation & Architecture
- [x] **Project Initialization**
    - [x] Verify ASP.NET Core MVC project structure
    - [ ] Remove default Bootstrap links from `_Layout.cshtml`
    - [ ] Remove default `site.css` and `bootstrap` folder from `wwwroot`
    - [ ] Add Tailwind CDN script to `_Layout.cshtml` `<head>`

- [ ] **Core Layout (Master Template)**
    - [ ] Extract **Head Section** (Fonts: Manrope, Material Symbols)
    - [ ] Extract **Sidebar Component** (Navigation Links)
    - [ ] Extract **Top Header Component** (Search, User Profile)
    - [ ] Implement `@RenderBody()` main content area
    - [ ] Verify Mobile Responsive Menu Toggle logic

## Phase 2: Feature Views Migration

### Module A: Dashboard (Home)
- [ ] **Setup**
    - [ ] Locate `Views/Home/Index.cshtml`
- [ ] **Content Migration**
    - [ ] Copy **KPI Cards** (Total Balance, Income, Expenses)
    - [ ] Copy **Chart Containers** (Cash Flow Trend)
    - [ ] Copy **Recent Transactions** Widget
- [ ] **Cleanup**
    - [ ] Remove default "Welcome" text from template

### Module B: System Alerts
- [ ] **Controller Setup**
    - [ ] Create `Controllers/AlertsController.cs`
    - [ ] Add `Index()` action method
- [ ] **View Implementation**
    - [ ] Create folder `Views/Alerts`
    - [ ] Create `Views/Alerts/Index.cshtml`
    - [ ] Copy **Filter Toolbar** (All, Critical, Warnings)
    - [ ] Copy **Alert List Items** (Critical, Warning, Info, Success)

### Module C: Transactions
- [ ] **Controller Setup**
    - [ ] Create `Controllers/TransactionsController.cs`
    - [ ] Add `Index()` action method
- [ ] **View Implementation**
    - [ ] Create folder `Views/Transactions`
    - [ ] Create `Views/Transactions/Index.cshtml`
    - [ ] Copy **Transaction Table** (Headers, Rows, Status Badges)
    - [ ] Copy **Pagination Controls**

### Module D: Financial Forecast
- [ ] **Controller Setup**
    - [ ] Create `Controllers/ForecastController.cs`
    - [ ] Add `Index()` action method
- [ ] **View Implementation**
    - [ ] Create folder `Views/Forecast`
    - [ ] Create `Views/Forecast/Index.cshtml`
    - [ ] Copy **Forecast Chart** container
    - [ ] Copy **Prediction Summary** cards

## Phase 3: Quality Assurance
- [ ] **Navigation Verification**
    - [ ] Test Sidebar "Dashboard" link -> `/`
    - [ ] Test Sidebar "Transactions" link -> `/Transactions`
    - [ ] Test Sidebar "Forecast" link -> `/Forecast`
    - [ ] Test Sidebar "System Alerts" link -> `/Alerts`
- [ ] **Visual Verification**
    - [ ] Check Mobile View (Hamburger menu, stacking)
    - [ ] Check Desktop View (Sidebar visibility)
