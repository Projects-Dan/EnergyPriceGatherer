namespace PowerPrices.Models;

public static class DanishTime
{
    private static readonly TimeZoneInfo DenmarkTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
    
    public static DateTimeOffset FromUtc(DateTimeOffset date)
    {
        DateTimeOffset convertedDate = TimeZoneInfo.ConvertTime(date, DenmarkTimeZone);
        return convertedDate;
    }
}