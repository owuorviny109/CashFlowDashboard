---
description: Sets up project guardrails including .editorconfig, strict compiler settings, and documentation structure.
---

# Setup Project Guardrails

This workflow aligns the project with strict professional standards for consistency and reliability.

## 1. Create .editorconfig
- [ ] Create a file named `.editorconfig` at the solution root (`d:\DOCUMENTS\PROJECTS\CashFlowDashboard\.editorconfig`).
- [ ] Add standard C# formatting rules (indentation = 4 spaces, utf-8, etc.).
- [ ] Enable code style analyzers.

## 2. Enforce Strict Compiler Settings
- [ ] Open `CashFlowDashboard\CashFlowDashboard.csproj`.
- [ ] Ensure the following settings are present in `<PropertyGroup>`:
    ```xml
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    ```

## 3. Setup Documentation Structure
- [ ] Create directory `Docs\adr` for Architecture Decision Records.
- [ ] Create a template or first ADR if ready.

## 4. Verification
- [ ] Run `dotnet format verify-no-changes` (or just `dotnet build` to check for warnings-as-errors).
