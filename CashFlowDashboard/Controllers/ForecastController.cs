using Microsoft.AspNetCore.Mvc;
using CashFlowDashboard.ViewModels;

namespace CashFlowDashboard.Controllers;

public class ForecastController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var model = new ForecastViewModel();
        return View(model);
    }
}
