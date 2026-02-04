using FinanceAggregator.Shared;
using System.Text.Json; // Need this for the options

namespace FinanceAggregator.Web;

public class FinanceApiClient(HttpClient httpClient)
{
    public async Task<List<Trade>> GetTradesAsync()
    {
        // 1. Configure options to ignore case (symbol vs Symbol)
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // 2. Fetch the data using these options
        var response = await httpClient.GetAsync("/trades");

        // 3. If the server errors, throw explicitly so the UI sees it
        response.EnsureSuccessStatusCode();

        // 4. Deserialize
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<Trade>>(content, options);

        return result ?? new List<Trade>();
    }
}