using PowerPrices.Models;

namespace PowerPrices.Data;

public interface IGetPrices
{
    Task<List<ElectricityPrice>> GetElectricityPricesAsync(DateOnly startDate);
}