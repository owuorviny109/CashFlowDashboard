using Microsoft.AspNetCore.Mvc;
using CashFlowDashboard.ViewModels;

namespace CashFlowDashboard.Controllers;

public class TransactionsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var model = new TransactionsViewModel();
        return View(model);
    }
}
