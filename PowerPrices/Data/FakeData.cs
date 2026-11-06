using PowerPrices.Models;

namespace PowerPrices.Data;

public static class FakeData
{
    private const decimal EurToDkk = 7.46m;
    private const int SlotsPerDay = 96; // 24 hours × 4 quarters

    // seed: same seed = same "random" prices every run (useful for testing)
    public static List<ElectricityPrice> ForDay(DateOnly date, int seed = 42)
    {
        var random = new Random(seed);
        // Simplification: the day starts at midnight UTC
        var start = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var prices = new List<ElectricityPrice>(SlotsPerDay * 2);

        for (int i = 0; i < SlotsPerDay; i++)
        {
            var time = start.AddMinutes(15 * i);
            decimal basePrice = BasePriceForHour(time.Hour);

            prices.Add(Create(time, PriceArea.DK1, basePrice, random));
            prices.Add(Create(time, PriceArea.DK2, basePrice * 1.08m, random)); // DK2 ~8% higher
        }

        return prices;
    }

    private static ElectricityPrice Create(
        DateTimeOffset time, PriceArea area, decimal basePrice, Random random)
    {
        decimal noise = (decimal)(random.NextDouble() * 0.2 - 0.1); // ±10%
        decimal dkk = Math.Round(basePrice * (1 + noise), 2);
        decimal eur = Math.Round(dkk / EurToDkk, 2);
        return new ElectricityPrice(time, area, dkk, eur);
    }

    private static decimal BasePriceForHour(int hour) => hour switch
    {
        < 6  => 400m,   // night: cheap
        < 9  => 900m,   // morning peak
        < 17 => 700m,   // daytime
        < 20 => 1600m,  // evening peak: expensive
        _    => 800m    // late evening
    };
}