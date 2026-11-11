namespace PowerPrices.Models;

public record ContWindow(DateTimeOffset Start, DateTimeOffset End, decimal Average)
{
    public string FormatContWindow()
    {
        return $"Start time: {DanishTime.FromUtc(Start):HH:mm} End time: {DanishTime.FromUtc(End):HH:mm}  Avg: {Average:F3} kr/kWh";
    }
}