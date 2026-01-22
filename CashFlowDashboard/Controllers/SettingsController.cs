using CashFlowDashboard.Data;
using CashFlowDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashFlowDashboard.Controllers;

public class SettingsController : Controller
{
    private readonly CashFlowDbContext _context;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(CashFlowDbContext context, ILogger<SettingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        // Auto-seed defaults if empty
        if (!await _context.AppSettings.AnyAsync())
        {
            await SeedDefaultsAsync();
        }

        var settings = await _context.AppSettings
            .OrderBy(s => s.Key)
            .ToListAsync();

        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Dictionary<string, string> settings)
    {
        try
        {
            foreach (var kvp in settings)
            {
                var setting = await _context.AppSettings.FindAsync(kvp.Key);
                if (setting != null)
                {
                    setting.Value = kvp.Value;
                }
            }
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Settings updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating settings");
            TempData["ErrorMessage"] = "Failed to update settings.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task SeedDefaultsAsync()
    {
        var defaults = new List<AppSetting>
        {
            new() { Key = "Alerts:LowBalanceThreshold", Value = "10000", Description = "Trigger alert when balance drops below this amount.", DataType = "decimal" },
            new() { Key = "Alerts:LargeTransactionThreshold", Value = "50000", Description = "Trigger alert for transactions exceeding this amount.", DataType = "decimal" },
            new() { Key = "Forecast:HistoryDays", Value = "90", Description = "Number of historical days to analyze for trends.", DataType = "int" },
            new() { Key = "Forecast:OptimisticMultiplier", Value = "1.2", Description = "Growth multiplier for Optimistic scenarios.", DataType = "decimal" },
            new() { Key = "Forecast:PessimisticMultiplier", Value = "0.8", Description = "Growth multiplier for Pessimistic scenarios.", DataType = "decimal" }
        };

        _context.AppSettings.AddRange(defaults);
        await _context.SaveChangesAsync();
    }
}
