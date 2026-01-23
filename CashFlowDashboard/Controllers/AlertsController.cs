using Microsoft.AspNetCore.Mvc;
using CashFlowDashboard.ViewModels;
using CashFlowDashboard.Services.Interfaces;

namespace CashFlowDashboard.Controllers;

public class AlertsController : Controller
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var alerts = await _alertService.GetActiveAlertsAsync(ct);

        // Group by time for better UX (Today/Yesterday/Older pattern)
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);

        var model = new AlertsViewModel
        {
            TodayAlerts = alerts.Where(a => a.GeneratedAt.Date == today).ToList(),
            YesterdayAlerts = alerts.Where(a => a.GeneratedAt.Date == yesterday).ToList(),
            OlderAlerts = alerts.Where(a => a.GeneratedAt.Date < yesterday).ToList(),
            UnreadCount = alerts.Count(a => a.Status == Models.Enums.AlertStatus.Unread)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct = default)
    {
        await _alertService.MarkAsReadAsync(id, ct);
        // PRG pattern to avoid double form submit
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct = default)
    {
        await _alertService.MarkAllAsReadAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Generate(CancellationToken ct = default)
    {
        await _alertService.GenerateSystemAlertsAsync(ct);
        return RedirectToAction(nameof(Index));
    }
}
