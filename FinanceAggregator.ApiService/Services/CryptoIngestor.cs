// In FinanceAggregator.ApiService/CryptoIngestor.cs

using FinanceAggregator.ApiService;
using FinanceAggregator.ApiService.Data;
using FinanceAggregator.Shared;
using System.Text.Json;

public class CryptoIngestor(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory, ILogger<CryptoIngestor> logger) : BackgroundService
{
    // 1. Define the coins we want to track
    private readonly string[] _coins = ["BTCUSDT", "ETHUSDT", "SOLUSDT", "XRPUSDT", "DOGEUSDT"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("🚀 Starting Multi-Coin Ingestor...");
        var httpClient = httpClientFactory.CreateClient("Binance");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

                // 2. Loop through each coin
                foreach (var symbol in _coins)
                {
                    try
                    {
                        var response = await httpClient.GetAsync($"/api/v3/ticker/price?symbol={symbol}", stoppingToken);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync(stoppingToken);
                            var ticker = JsonSerializer.Deserialize<BinanceTicker>(content);

                            if (ticker != null && decimal.TryParse(ticker.Price, out var currentPrice))
                            {
                                // Convert "ETHUSDT" -> "ETH-USD" for display
                                var displaySymbol = symbol.Replace("USDT", "-USD");

                                db.Trades.Add(new Trade
                                {
                                    Symbol = displaySymbol,
                                    Price = currentPrice,
                                    Volume = 0,
                                    Timestamp = DateTime.UtcNow
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Error fetching {Symbol}: {Message}", symbol, ex.Message);
                    }

                    // Small delay between requests to be nice to Binance API
                    await Task.Delay(200, stoppingToken);
                }

                // Save all changes at once
                await db.SaveChangesAsync(stoppingToken);
            }

            // Wait 5 seconds before next batch (prevents rate limiting)
            await Task.Delay(5000, stoppingToken);
        }
    }
}