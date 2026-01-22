using Microsoft.EntityFrameworkCore;
using CashFlowDashboard.Data.Repositories;
using CashFlowDashboard.Data.Repositories.Interfaces;

namespace CashFlowDashboard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register Entity Framework Core DbContext
            builder.Services.AddDbContext<CashFlowDashboard.Data.CashFlowDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<IAlertRepository, AlertRepository>();
            builder.Services.AddScoped<ICashFlowSnapshotRepository, CashFlowSnapshotRepository>();
            builder.Services.AddScoped<IForecastRepository, ForecastRepository>();

            // Register Services
            builder.Services.AddScoped<CashFlowDashboard.Services.Interfaces.ITransactionService, CashFlowDashboard.Services.TransactionService>();
            builder.Services.AddScoped<CashFlowDashboard.Services.Interfaces.IAlertService, CashFlowDashboard.Services.AlertService>();
            builder.Services.AddScoped<CashFlowDashboard.Services.Interfaces.ICashFlowAnalyticsService, CashFlowDashboard.Services.CashFlowAnalyticsService>();
            builder.Services.AddScoped<CashFlowDashboard.Services.Interfaces.IForecastService, CashFlowDashboard.Services.ForecastService>();

            // Register Configuration
            builder.Services.Configure<CashFlowDashboard.Configuration.ForecastSettings>(builder.Configuration.GetSection("Forecast"));
            builder.Services.Configure<CashFlowDashboard.Configuration.AlertSettings>(builder.Configuration.GetSection("Alerts"));

            var app = builder.Build();

            // Seed Database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<CashFlowDashboard.Data.CashFlowDbContext>();
                    // Ensure database is created/migrated
                    await context.Database.MigrateAsync();
                    // Seed initial data
                    await CashFlowDashboard.Data.DataSeeder.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
