using FinanceAggregator.ApiService.Data;
using FinanceAggregator.ApiService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// In FinanceAggregator.ApiService/Program.cs

builder.AddNpgsqlDbContext<FinanceDbContext>("FinanceDb",
    // 1. Force Npgsql settings to disable SSL
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.ConnectionString += ";SSL Mode=Disable;Include Error Detail=true";
    },
    // 2. Ensure EF Core options also respect this (optional safety net)
    configureDbContextOptions: options =>
    {
        // keeps the context options clean
    });

builder.Services.AddHostedService<MarketSimulator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

    // Wait briefly for the Aspire Postgres container to finish initializing
    await Task.Delay(3000);

    // Use MigrateAsync instead of EnsureCreatedAsync
    // This will successfully add tables to the existing "FinanceDb"
    await db.Database.MigrateAsync();
}

app.MapGet("/trades", async (FinanceDbContext db) =>
{
    return await db.Trades
                   .OrderByDescending(t => t.Timestamp)
                   .Take(50)
                   .ToListAsync();
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
