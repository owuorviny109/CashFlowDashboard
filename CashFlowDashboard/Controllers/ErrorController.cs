using System.Diagnostics;
using CashFlowDashboard.Models;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowDashboard.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error page displayed. Request ID: {RequestId}", requestId);
        
        return View("Error", new ErrorViewModel { RequestId = requestId });
    }

    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogWarning("HTTP Error {StatusCode}. Request ID: {RequestId}", statusCode, requestId);

        switch (statusCode)
        {
            case 404:
                ViewData["Title"] = "Page Not Found";
                ViewData["ErrorMessage"] = "Sorry, the page you are looking for does not exist.";
                break;
            default:
                ViewData["Title"] = "Error";
                ViewData["ErrorMessage"] = "An unexpected error occurred.";
                break;
        }

        return View("Error", new ErrorViewModel { RequestId = requestId });
    }
}
