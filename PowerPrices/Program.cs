using System.Reflection.Emit;
using PowerPrices.Models;
using PowerPrices.Data;
using PowerPrices.Services;

var prices = FakeData.ForDay(new DateOnly(2026, 11, 6));
Console.WriteLine($"Count: {prices.Count}");

foreach (var p in prices.Where(p => p.Area == PriceArea.DK1).Take(8))
{
    Console.WriteLine($"{p.TimeUtc:HH:mm}  {p.PriceDkkPerKwh:F2} kr/kWh");
}

var analysePrices = new PriceAnalyzer();

foreach (PriceArea area in Enum.GetValues(typeof(PriceArea)))
{
    Console.WriteLine($"==={area}===");
    Print("Cheapest:",analysePrices.FindCheapest(prices, area));
    Print("Most expensive:",analysePrices.FindMostExpensive(prices, area));
    var average = analysePrices.FindAverage(prices, area);
    Console.WriteLine($"Average: {average?.ToString("F3") ?? "No data"} kr/kWh");
}

Console.WriteLine(analysePrices.CheapestContWindow(prices, PriceArea.DK1, TimeSpan.FromHours(3.5))?.FormatContWindow());

void Print(string label, ElectricityPrice? price)
{
    if (price is null)
    {
        Console.WriteLine($"{label} No data");
        return;
    }
    Console.WriteLine($"{label} {price.FormatTimePrice()}");
}

// ===== TESTS: CheapestContWindow =====
var hourly = TimeSpan.FromHours(1);
var quarter = TimeSpan.FromMinutes(15);

Test("1. Basic",         MakePrices([5, 4, 1, 1, 1, 9], hourly), TimeSpan.FromHours(3));
Test("2. Unsorted",      MakePrices([5, 4, 1, 1, 1, 9], hourly).OrderByDescending(p => p.TimeUtc).ToList(), TimeSpan.FromHours(3));
Test("3. One price",     MakePrices([5], hourly), TimeSpan.FromHours(1));
Test("4. Window = data", MakePrices([5, 4, 1, 1, 1, 9], hourly), TimeSpan.FromHours(6));
Test("5. Window > data", MakePrices([5, 4, 1, 1, 1, 9], hourly), TimeSpan.FromHours(7));
Test("6. 15-min data",   MakePrices([3, 3, 3, 3, 1, 1, 1, 1, 2, 2, 2, 2], quarter), TimeSpan.FromHours(1));
Test("7. Wrong area",    MakePrices([5, 4, 1, 1, 1, 9], hourly), TimeSpan.FromHours(3), PriceArea.DK2);
Test("8. Cheapest last", MakePrices([9, 9, 9, 1, 1, 1], hourly), TimeSpan.FromHours(3));
Test("9. Hourly, 3.5h window",  MakePrices([5, 4, 1, 1, 1, 9], hourly), TimeSpan.FromHours(3.5));
Test("10. 15-min, 1.5h window", MakePrices([3, 3, 3, 3, 1, 1, 1, 1, 1, 1, 2, 2], quarter), TimeSpan.FromHours(1.5));
Test("11. Tie → first wins", MakePrices([1, 1, 1, 9, 1, 1, 1], hourly), TimeSpan.FromHours(3));

List<ElectricityPrice> MakePrices(decimal[] kwhPrices, TimeSpan interval)
{
    var start = new DateTimeOffset(2026, 11, 8, 0, 0, 0, TimeSpan.Zero);
    return kwhPrices
        .Select((price, i) => new ElectricityPrice(
            start + interval * i, PriceArea.DK1, price * 1000m, price * 1000m / 7.46m))
        .ToList();
}

void Test(string name, List<ElectricityPrice> data, TimeSpan window, PriceArea area = PriceArea.DK1)
{
    var result = analysePrices.CheapestContWindow(data, area, window);
    Console.WriteLine($"{name}: {result?.FormatContWindow() ?? "null"}");
}

