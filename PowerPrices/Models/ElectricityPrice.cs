namespace PowerPrices.Models;

public record ElectricityPrice(
    DateTimeOffset TimeUtc,
    PriceArea Area,
    decimal PriceDkkPerMwh,
    decimal PriceEurPerMwh)
{
    public decimal PriceDkkPerKwh => PriceDkkPerMwh / 1000m;
    public decimal PriceEurPerKwh => PriceEurPerMwh / 1000m;
}