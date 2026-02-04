using FinanceAggregator.ApiService.Data;
using FinanceAggregator.ApiService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services.AddHttpClient("Binance", client =>
{
    client.BaseAddress = new Uri("https://api.binance.com");
    client.DefaultRequestHeaders.Add("User-Agent", "FinanceAggregator/1.0");
});

builder.AddNpgsqlDbContext<FinanceDbContext>("FinanceDb", configureSettings: settings =>
{
    // 1. Force Disable SSL (The "Hammer" approach)
    settings.ConnectionString += ";SSL Mode=Disable;Trust Server Certificate=true";

    // 2. Enable detailed errors so we see WHY it fails next time
    settings.ConnectionString += ";Include Error Detail=true";
});

builder.Services.AddHostedService<CryptoIngestor>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

    await Task.Delay(3000);

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