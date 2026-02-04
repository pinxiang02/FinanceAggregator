using System.Text.Json.Serialization;

namespace FinanceAggregator.ApiService;

public class BinanceTicker
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;
}