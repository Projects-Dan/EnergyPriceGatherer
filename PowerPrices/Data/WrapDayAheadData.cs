namespace PowerPrices.Data;

public class WrapDayAheadData
{
    public WrapDayAheadData()
    {
        Records = [];
    }
    public List<DayAheadData> Records { get; init; }
}