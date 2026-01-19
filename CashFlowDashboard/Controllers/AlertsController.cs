using Microsoft.AspNetCore.Mvc;
using CashFlowDashboard.ViewModels;

namespace CashFlowDashboard.Controllers;

public class AlertsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var model = new AlertsViewModel();
        return View(model);
    }
}
