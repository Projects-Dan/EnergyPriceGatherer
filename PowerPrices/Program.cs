using System.Net;
using System.Text.Json;
using PowerPrices.Models;
using PowerPrices.Data;
using PowerPrices.Services;

List<ElectricityPrice> electricityPrices;
DateOnly today = DateOnly.FromDateTime(DateTime.Now);
DateOnly tomorrow = today.AddDays(1);
DateOnly dayAfterTomorrow = tomorrow.AddDays(1);

IGetPrices getPrices = new ApiClient();

try
{
    electricityPrices = await getPrices.GetElectricityPricesAsync(tomorrow);
}
catch (HttpRequestException e) when (e.StatusCode is null)
{
    Console.WriteLine("No internet connection or server unreachable");
    return;
}
catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.TooManyRequests)
{
    Console.WriteLine("The API is busy, try again shortly");
    return;
}
catch (HttpRequestException e)
{
    Console.WriteLine(e.Message);
    return;
}
catch (InvalidOperationException e)
{
    Console.WriteLine(e.Message);
    return;
}
catch (ArgumentOutOfRangeException e)
{
    Console.WriteLine(e.Message);
    return;
}
catch (JsonException e)
{
    Console.WriteLine(e.Message);
    return;
}

var analysePrices = new PriceAnalyzer();

if (!electricityPrices.Any())
{
    Console.WriteLine($"Prices for {tomorrow:dd-MM} have not been published yet. Try on {tomorrow.AddDays(-1):dd-MM} again after 13:00.");
    return;
}

TimeSpan window = TimeSpan.FromHours(3.5);
foreach (PriceArea area in Enum.GetValues(typeof(PriceArea)))
{
    Console.WriteLine($"==={area}===");
    Print("Cheapest:", analysePrices.FindCheapest(electricityPrices, area));
    Print("Most expensive:", analysePrices.FindMostExpensive(electricityPrices, area));
    var average = analysePrices.FindAverage(electricityPrices, area);
    Console.WriteLine($"Average: {average?.ToString("F3") ?? "No data"} kr/kWh");
    var tempContWindow = analysePrices.CheapestContWindow(electricityPrices, area, window);
    Console.WriteLine($"Cheapest {window.TotalHours} hour window: {tempContWindow?.FormatContWindow() ?? "No data"}");
}

void Print(string label, ElectricityPrice? price)
{
    if (price is null)
    {
        Console.WriteLine($"{label} No data");
        return;
    }
    Console.WriteLine($"{label} {price.FormatTimePrice()}");
}



