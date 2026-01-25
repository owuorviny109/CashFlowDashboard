---
description: Scaffolds a new feature module (vertical slice) with Controller, ViewModel, View, and Service Interface.
---

# Scaffold Vertical Slice (Feature)

Use this workflow to start a new feature module (e.g., Dashboard, Reports).

## Input
- **FeatureName**: (e.g., `Reports`)

## Steps

### 1. Create ViewModel
- [ ] Create `ViewModels/[FeatureName]ViewModel.cs`.
- [ ] Content:
```csharp
namespace CashFlowDashboard.ViewModels;

public sealed class [FeatureName]ViewModel
{
    // Add properties here (e.g., public string Title { get; init; })
}
```

### 2. Create Controller
- [ ] Create `Controllers/[FeatureName]Controller.cs`.
- [ ] Content:
```csharp
using Microsoft.AspNetCore.Mvc;
using CashFlowDashboard.ViewModels;

namespace CashFlowDashboard.Controllers;

public class [FeatureName]Controller : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var model = new [FeatureName]ViewModel();
        return View(model);
    }
}
```

### 3. Create View
- [ ] Create directory `Views/[FeatureName]`.
- [ ] Create `Views/[FeatureName]/Index.cshtml`.
- [ ] Content:
```cshtml
@model CashFlowDashboard.ViewModels.[FeatureName]ViewModel

@{
    ViewData["Title"] = "[FeatureName]";
}

<div class="p-6">
    <h1 class="text-2xl font-bold mb-4">@ViewData["Title"]</h1>
    <!-- Content -->
</div>
```

### 4. Create Service Interface (Optional)
- [ ] Create `Services/I[FeatureName]Service.cs` if business logic is needed.
