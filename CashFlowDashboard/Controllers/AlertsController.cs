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
    [HttpGet]
    public async Task<IActionResult> Index(string filter = "All", int page = 1, CancellationToken ct = default)
    {
        var alerts = await _alertService.GetActiveAlertsAsync(ct);

        // Apply visual filtering
        if (filter == "Critical")
        {
            alerts = alerts.Where(a => a.Severity == Models.Enums.AlertSeverity.Critical).ToList();
        }
        else if (filter == "Warning")
        {
            alerts = alerts.Where(a => a.Severity == Models.Enums.AlertSeverity.Warning).ToList();
        }

        // Pagination
        int pageSize = 10;
        int totalCount = alerts.Count;
        var pagedAlerts = alerts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Group the PAGED results by time
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);

        var model = new AlertsViewModel
        {
            TodayAlerts = pagedAlerts.Where(a => a.GeneratedAt.Date == today).ToList(),
            YesterdayAlerts = pagedAlerts.Where(a => a.GeneratedAt.Date == yesterday).ToList(),
            OlderAlerts = pagedAlerts.Where(a => a.GeneratedAt.Date < yesterday).ToList(),
            UnreadCount = alerts.Count(a => a.Status == Models.Enums.AlertStatus.Unread), // Count from total, not paged
            CurrentFilter = filter,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize
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
