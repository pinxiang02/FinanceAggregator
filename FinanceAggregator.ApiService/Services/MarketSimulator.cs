using FinanceAggregator.Shared;
using FinanceAggregator.ApiService.Data;

namespace FinanceAggregator.ApiService.Services;

public class MarketSimulator(IServiceProvider serviceProvider, ILogger<MarketSimulator> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting Market Simulator...");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Create a new scope to retrieve the Database Context
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

                // 1. Create a random trade
                var trade = new Trade
                {
                    Symbol = "BTC-USD",
                    Price = 96000m + (decimal)(Random.Shared.NextDouble() * 500 - 250), // Random price fluctuation
                    Volume = (decimal)Random.Shared.NextDouble() * 2m,
                    Timestamp = DateTime.UtcNow
                };

                // 2. Save to Postgres
                db.Trades.Add(trade);
                await db.SaveChangesAsync(stoppingToken);

                logger.LogInformation("📈 Generated Trade: {Symbol} at ${Price:F2}", trade.Symbol, trade.Price);
            }

            // Wait 1 second before the next trade
            await Task.Delay(1000, stoppingToken);
        }
    }
}