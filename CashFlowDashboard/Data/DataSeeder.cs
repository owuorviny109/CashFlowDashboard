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

        var transactions = new List<Transaction>();
        var random = new Random(42); // Deterministic seed for consistent data

        // Generate 6 months of historical transactions for chart visualization
        var startDate = DateTime.Today.AddMonths(-6);
        var currentDate = startDate;

        while (currentDate <= DateTime.Today)
        {
            // Generate 2-5 transactions per day
            var transactionsPerDay = random.Next(2, 6);

            for (int i = 0; i < transactionsPerDay; i++)
            {
                var isIncome = random.Next(0, 100) > 40; // 60% chance of income
                var amount = isIncome 
                    ? random.Next(1000, 15000) // Income: $1k-$15k
                    : random.Next(200, 5000);  // Expense: $200-$5k

                var type = isIncome ? TransactionType.Income : TransactionType.Expense;
                var category = isIncome 
                    ? new[] { "Sales", "Consulting", "Investment", "Services" }[random.Next(4)]
                    : new[] { "Software", "Office", "Marketing", "Payroll", "Utilities" }[random.Next(5)];
                
                var description = isIncome
                    ? $"Payment from {new[] { "Acme Corp", "Global Inc", "Tech Solutions", "Enterprise LLC" }[random.Next(4)]}"
                    : $"{category} expense - {new[] { "Monthly", "Quarterly", "Annual", "One-time" }[random.Next(4)]}";

                transactions.Add(new Transaction
                {
                    Id = Guid.NewGuid(),
                    Date = currentDate.AddHours(random.Next(8, 18)), // Business hours
                    Amount = amount,
                    Type = type,
                    Category = category,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                });
            }

            currentDate = currentDate.AddDays(1);
        }

        // Add alerts
        var alerts = new List<Alert>
        {
            new() { Id = Guid.NewGuid(), Severity = AlertSeverity.Warning, Title = "Low Balance Projection", Message = "Projected balance drops below threshold in 15 days.", Category = AlertCategory.Forecast, Status = AlertStatus.Unread, GeneratedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Severity = AlertSeverity.Critical, Title = "Overdue Invoice", Message = "Invoice #1023 is 5 days overdue.", Category = AlertCategory.Invoice, Status = AlertStatus.Unread, GeneratedAt = DateTime.UtcNow.AddHours(-4) },
            new() { Id = Guid.NewGuid(), Severity = AlertSeverity.Info, Title = "Tax Deadline Approaching", Message = "Quarterly tax payment due in 7 days.", Category = AlertCategory.Forecast, Status = AlertStatus.Read, GeneratedAt = DateTime.UtcNow.AddDays(-2) }
        };

        await context.Transactions.AddRangeAsync(transactions);
        await context.Alerts.AddRangeAsync(alerts);
        await context.SaveChangesAsync();

        // Generate daily snapshots for chart visualization
        await GenerateSnapshotsAsync(context, startDate, DateTime.Today);
    }

    private static async Task GenerateSnapshotsAsync(CashFlowDbContext context, DateTime startDate, DateTime endDate)
    {
        var snapshots = new List<CashFlowSnapshot>();
        decimal openingBalance = 50000; // Must match the starting balance assumption

        var currentDate = DateOnly.FromDateTime(startDate);
        var endDateOnly = DateOnly.FromDateTime(endDate);

        while (currentDate <= endDateOnly)
        {
            var dayStart = currentDate.ToDateTime(TimeOnly.MinValue);
            var dayEnd = currentDate.ToDateTime(TimeOnly.MaxValue);

            // Get all transactions for this day (already loaded in memory from seeding)
            var dayTransactions = await context.Transactions
                .Where(t => t.Date >= dayStart && t.Date <= dayEnd)
                .ToListAsync();

            var totalIncome = dayTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => (decimal?)t.Amount) ?? 0;
            var totalExpenses = dayTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => (decimal?)t.Amount) ?? 0;
            var netCashFlow = totalIncome - totalExpenses;
            var closingBalance = openingBalance + netCashFlow;

            snapshots.Add(new CashFlowSnapshot
            {
                Id = Guid.NewGuid(),
                Date = currentDate,
                OpeningBalance = openingBalance,
                ClosingBalance = closingBalance,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                NetCashFlow = netCashFlow,
                TransactionCount = dayTransactions.Count,
                ComputedAt = DateTime.UtcNow
            });

            openingBalance = closingBalance; // Next day's opening = today's closing
            currentDate = currentDate.AddDays(1);
        }

        await context.Snapshots.AddRangeAsync(snapshots);
        await context.SaveChangesAsync();
    }
}
