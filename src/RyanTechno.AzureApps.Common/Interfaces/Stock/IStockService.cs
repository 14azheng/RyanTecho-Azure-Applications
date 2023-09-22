using RyanTechno.AzureApps.Domain.Stock;

namespace RyanTechno.AzureApps.Common.Interfaces.Stock
{
    public interface IStockService
    {
        Task<List<StockDaily>?> GetDailyStockInfoAsync(HttpClient httpClient, string requestUrl);
    }
}
