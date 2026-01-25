using Microsoft.AspNetCore.Mvc;
using CashFlowDashboard.ViewModels;
using CashFlowDashboard.Services.Interfaces;
using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.Controllers;

public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> Index(
        string? search, 
        string? type, 
        DateTime? start, 
        DateTime? end, 
        int page = 1, 
        int pageSize = 10, 
        CancellationToken ct = default)
    {
        // Parse Enum
        Models.Enums.TransactionType? typeEnum = null;
        if (!string.IsNullOrEmpty(type) && Enum.TryParse<Models.Enums.TransactionType>(type, true, out var parsedType))
        {
            typeEnum = parsedType;
        }

        // defaults if not provided (optional logic, but let's keep it wide open if null)
        // actually, if start/end are null, we search all time.

        var (transactions, summary) = await _transactionService.GetFilteredTransactionsAsync(
            search, 
            typeEnum, 
            start, 
            end, 
            ct);
        
        // In-memory pagination of the filtered result
        var paginatedTransactions = transactions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new TransactionsViewModel
        {
            Transactions = paginatedTransactions,
            Summary = summary,
            SearchTerm = search,
            TypeFilter = type,
            StartDate = start,
            EndDate = end,
            TotalCount = transactions.Count,
            CurrentPage = page,
            PageSize = pageSize
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionCommand command, CancellationToken ct = default)
    {
        // ... (existing validation logic)
        
        try 
        {
            await _transactionService.CreateTransactionAsync(command, ct);
            TempData["SuccessMessage"] = "Transaction added successfully!";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Transaction Create Exception] {ex.Message}");
            TempData["ErrorMessage"] = $"Error creating transaction: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateTransactionCommand command, CancellationToken ct = default)
    {
        // We do manual binding/checking since UpdateTransactionCommand is for patches usually
        // But here we can reuse it or create a new one. 
        // For simplicity, we assume the form sends compatible fields.

        try
        {
            await _transactionService.UpdateTransactionAsync(id, command, ct);
            TempData["SuccessMessage"] = "Transaction updated successfully!";
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Failed to update transaction {Id}", id);
            TempData["ErrorMessage"] = $"Error updating transaction: {ex.Message}";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await _transactionService.DeleteTransactionAsync(id, ct);
        TempData["SuccessMessage"] = "Transaction deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
