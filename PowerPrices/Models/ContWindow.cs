namespace PowerPrices.Models;

public record ContWindow(DateTimeOffset Start, DateTimeOffset End, decimal Average)
{
    public string FormatContWindow()
    {
        return $"Start time: {Start:HH:mm} End time: {End:HH:mm}  Avg: {Average:F3} kr/kWh";
    }
};