using CashFlowDashboard.Models.Entities;
using CashFlowDashboard.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CashFlowDashboard.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(CashFlowDbContext context)
    {
        if (await context.Transactions.AnyAsync())
        {
            return; // Already seeded
        }

        var transactions = new List<Transaction>
        {
            new() { Id = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-2), Amount = 5000, Type = TransactionType.Income, Category = "Sales", Description = "Client Payment - Acme Corp", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-5), Amount = 150, Type = TransactionType.Expense, Category = "Software", Description = "Cloud Hosting", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-10), Amount = 2000, Type = TransactionType.Income, Category = "Consulting", Description = "Tech Consultation", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-1), Amount = 1200, Type = TransactionType.Expense, Category = "Office", Description = "Office Rent", CreatedAt = DateTime.UtcNow }
        };

        var alerts = new List<Alert>
        {
            new() { Id = Guid.NewGuid(), Severity = AlertSeverity.Warning, Title = "Low Balance Projection", Message = "Projected balance drops below threshold in 15 days.", Category = AlertCategory.Forecast, Status = AlertStatus.Unread, GeneratedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Severity = AlertSeverity.Critical, Title = "Overdue Invoice", Message = "Invoice #1023 is 5 days overdue.", Category = AlertCategory.Invoice, Status = AlertStatus.Unread, GeneratedAt = DateTime.UtcNow.AddHours(-4) }
        };

        await context.Transactions.AddRangeAsync(transactions);
        await context.Alerts.AddRangeAsync(alerts);
        await context.SaveChangesAsync();
    }
}
