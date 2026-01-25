using Microsoft.EntityFrameworkCore;
using CashFlowDashboard.Data.Repositories;
using CashFlowDashboard.Data.Repositories.Interfaces;

using Serilog;

namespace CashFlowDashboard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Initialize Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting web application");

                var builder = WebApplication.CreateBuilder(args);
                
                // Force US Culture for Currency (USD)
                var cultureInfo = new System.Globalization.CultureInfo("en-US");
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                // Use Serilog for logging
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllersWithViews(options =>
                {
                    // Register global exception filter for service-layer exceptions
                    options.Filters.Add<CashFlowDashboard.Filters.GlobalExceptionFilter>();
                });

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
                        // Use static Log class here as app logger might not be ready if DI fails
                        Log.Error(ex, "An error occurred while seeding the database.");
                    }
                }

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    app.UseStatusCodePagesWithReExecute("/Error/{0}");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                
                // Add Serilog Request Logging (captures HTTP info)
                app.UseSerilogRequestLogging();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
