using PowerPrices.Models;

var p = new ElectricityPrice(DateTimeOffset.UtcNow, PriceArea.DK1, 850m, 114m);
Console.WriteLine(p);
Console.WriteLine(p.PriceDkkPerKwh);