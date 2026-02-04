using FinanceAggregator.Shared;
using FinanceAggregator.ApiService.Data;
using System.Text.Json;

namespace FinanceAggregator.ApiService.Services;

public class CryptoIngestor(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory, ILogger<CryptoIngestor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("🚀 Starting Real-Time Crypto Ingestor...");

        var httpClient = httpClientFactory.CreateClient("Binance");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. Fetch real data from Binance (BTCUSDT)
                // API Docs: https://binance-docs.github.io/apidocs/spot/en/#symbol-price-ticker
                var response = await httpClient.GetAsync("/api/v3/ticker/price?symbol=BTCUSDT", stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(stoppingToken);
                    var ticker = JsonSerializer.Deserialize<BinanceTicker>(content);

                    if (ticker != null && decimal.TryParse(ticker.Price, out var currentPrice))
                    {
                        // 2. Save to Database
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

                            var trade = new Trade
                            {
                                Symbol = "BTC-USD", // Standardize name for your app
                                Price = currentPrice,
                                Volume = 0, // Ticker endpoint doesn't give volume, defaulting to 0
                                Timestamp = DateTime.UtcNow
                            };

                            db.Trades.Add(trade);
                            await db.SaveChangesAsync(stoppingToken);

                            logger.LogInformation("✅ BTC: ${Price}", trade.Price);
                        }
                    }
                }
                else
                {
                    logger.LogWarning("⚠️ Binance API failed: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("❌ Error fetching crypto data: {Message}", ex.Message);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}