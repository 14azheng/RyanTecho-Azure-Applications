using RyanTechno.AzureApps.Common.Models.Stock;
using RyanTechno.AzureApps.Domain.Stock;

namespace RyanTechno.AzureApps.Services.Extensions
{
    public static class StockExtension
    {
        public static List<StockDaily>? ToModels(this DailyStockApiStructure dailyStockApiStructure) => dailyStockApiStructure.TimeSeries?.Select(d => new StockDaily()
        {
            Date = d.Key,
            Close = d.Value.Close,
            High = d.Value.High,
            Low = d.Value.Low,
            Open = d.Value.Open,
            Volume = d.Value.Volume,
        }).OrderBy(s => s.Date).ToList();
    }
}
