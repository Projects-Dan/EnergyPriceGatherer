using PowerPrices.Models;

namespace PowerPrices.Data;

public static class ConvertData
{ 
    public static List<ElectricityPrice> ConvertDataToElectricityPrices(WrapDayAheadData data)
    {
        return  data.Records.Select(Create).ToList();
    }

    private static ElectricityPrice Create(DayAheadData data)
    {
        PriceArea tempArea;
        DateTimeOffset tempTime = new DateTimeOffset(DateTime.SpecifyKind(data.TimeUtc, DateTimeKind.Utc)); // Convert DateTime to DateTimeOffset and specify that it is in Utc.

        tempArea = data.PriceArea switch
        {
            "DK1" => PriceArea.DK1,
            "DK2" => PriceArea.DK2,
            _ => throw new ArgumentOutOfRangeException($"Not expected PriceArea: {data.PriceArea}")
        };
        
        return new ElectricityPrice(tempTime, tempArea, data.DayAheadPriceDkk,  data.DayAheadPriceEur);
    }
}