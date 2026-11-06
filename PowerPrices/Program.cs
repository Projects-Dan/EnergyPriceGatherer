using PowerPrices.Models;
using PowerPrices.Data;

var prices = FakeData.ForDay(new DateOnly(2026, 11, 6));
Console.WriteLine($"Count: {prices.Count}");

foreach (var p in prices.Where(p => p.Area == PriceArea.DK1).Take(8))
{
    Console.WriteLine($"{p.TimeUtc:HH:mm}  {p.PriceDkkPerKwh:F2} kr/kWh");
}
