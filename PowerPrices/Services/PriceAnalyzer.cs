using PowerPrices.Models;
namespace PowerPrices.Services;

public class PriceAnalyzer
{
    public ElectricityPrice? FindCheapest(IEnumerable<ElectricityPrice> elPrices, PriceArea area)
    {
        IEnumerable<ElectricityPrice> query = FilterByArea(elPrices, area);
        
        if (!query.Any())
        {
            return null;
        }
        
        return query.MinBy(price => price.PriceDkkPerKwh);
    }
    
    public ElectricityPrice? FindMostExpensive(IEnumerable<ElectricityPrice> elPrices, PriceArea area)
    {
        IEnumerable<ElectricityPrice> query = FilterByArea(elPrices, area);
        
        if (!query.Any() || elPrices is null)
        {
            return null;
        }
        
        return query.MaxBy(price => price.PriceDkkPerKwh);
    }
    public decimal? FindAverage(IEnumerable<ElectricityPrice> elPrices, PriceArea area)
    {
        IEnumerable<ElectricityPrice> query = FilterByArea(elPrices, area);
        if (!query.Any())
        {
            return null;
        }
        return query.Average(price => price.PriceDkkPerKwh);
    }

    public ContWindow? CheapestContWindow(IEnumerable<ElectricityPrice> elPrices, PriceArea area, TimeSpan window)
    {
        elPrices = elPrices.OrderBy(time => time.TimeUtc); // Ensure that the list is ordered based on time
        
        IEnumerable<ElectricityPrice> query = FilterByArea(elPrices, area);
        
        int n = query.Count();
        TimeSpan? diff = n >= 2 ? query.ElementAt(1).TimeUtc - query.ElementAt(0).TimeUtc : null;
        int slots = diff.HasValue ? (int)Math.Floor(window / diff.Value) : 0;
        
        if (n < slots || slots <= 0)
        {
            return null;
        }
        
        DateTimeOffset startTime = query.ElementAt(0).TimeUtc;
        
        decimal minSum = query.Take(slots).Sum(price => price.PriceDkkPerKwh);
        decimal windowSum = minSum;
        for (int i = slots; i < n; i++)
        {
            windowSum += query.ElementAt(i).PriceDkkPerKwh-query.ElementAt(i-slots).PriceDkkPerKwh;
            if (windowSum < minSum)
            {
                minSum = windowSum;
                startTime = query.ElementAt(i-slots+1).TimeUtc; // window now covers i-slots+1 ... i
            }
        }
        var contWindow = new ContWindow(startTime, startTime + diff.Value*slots, minSum/slots);
        return contWindow;
    }

    private List<ElectricityPrice> FilterByArea(IEnumerable<ElectricityPrice> elPrices, PriceArea area)
    {
        return elPrices.Where(price => price.Area == area).ToList();
    }
}