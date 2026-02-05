using FinanceAggregator.Shared;
using System.Text.Json;

namespace FinanceAggregator.Web;

public class FinanceApiClient(HttpClient httpClient)
{
    public async Task<List<Trade>> GetTradesAsync(string? symbol = null)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var url = "/trades";
        if (!string.IsNullOrEmpty(symbol))
        {
            url += $"?symbol={symbol}";
        }

        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<Trade>>(content, options);

        return result ?? new List<Trade>();
    }
}