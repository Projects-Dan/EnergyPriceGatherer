namespace PowerPrices.Data;

public record DayAheadData
{
    private DateTime _timeUtc;
    public required DateTime TimeUtc
    {
        get => _timeUtc;
        init => _timeUtc = value;
    }

    public required string PriceArea { get; set; }
    public decimal DayAheadPriceEur { get; set; }
    public decimal DayAheadPriceDkk { get; set; }
}