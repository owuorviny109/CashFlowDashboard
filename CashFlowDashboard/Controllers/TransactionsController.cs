using Microsoft.AspNetCore.Mvc;
using CashFlowDashboard.ViewModels;
using CashFlowDashboard.Services.Interfaces;
using CashFlowDashboard.Services.DTOs;

namespace CashFlowDashboard.Controllers;

public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        // Basic pagination. For production, use repository-level limit/offset.
        var allTransactions = await _transactionService.GetRecentTransactionsAsync(1000, ct);
        
        var paginatedTransactions = allTransactions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new TransactionsViewModel
        {
            Transactions = paginatedTransactions,
            TotalCount = allTransactions.Count,
            CurrentPage = page,
            PageSize = pageSize
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionCommand command, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            
            Console.WriteLine($"[Transaction Create Failed] Validation Errors: {errors}");
            TempData["ErrorMessage"] = $"Could not create transaction: {errors}";
            
            return RedirectToAction(nameof(Index));
        }

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
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await _transactionService.DeleteTransactionAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
