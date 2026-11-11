using System.Globalization;
using System.Text.Json;
using PowerPrices.Models;

namespace PowerPrices.Data;

public class ApiClient : IGetPrices
{
    private static readonly HttpClient Client = new HttpClient();
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };
    private const string BaseUrl= "https://api.energidataservice.dk";

    public async Task<List<ElectricityPrice>> GetElectricityPricesAsync(DateOnly startDate)
    {
        
       string response = await GetJsonOneDayAsync(startDate);
       var dayAheadData = JsonSerializer.Deserialize<WrapDayAheadData>(response, Options);
       if  (dayAheadData is null)
           throw new InvalidOperationException("The API returned no data");
       
       return ConvertData.ConvertDataToElectricityPrices(dayAheadData);
    }
    private async Task<string> GetJsonOneDayAsync(DateOnly startDate)
    {
        DateOnly endDate = startDate.AddDays(1);

        string url = $"{BaseUrl}/dataset/DayAheadPrices?start={startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}" +
              $"&end={endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&filter={Uri.EscapeDataString("""{"PriceArea":["DK1","DK2"]}""")}" +
              $"&sort={Uri.EscapeDataString("TimeUTC asc")}";
        using HttpResponseMessage response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode(); 
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}