using RyanTechno.AzureApps.Common.Interfaces.Stock;
using RyanTechno.AzureApps.Infrastructure.Logging;
using RyanTechno.AzureApps.Services.Network;
using RyanTechno.AzureApps.Services.Stock;

namespace RyanTechno.AzureApps.Services.UnitTest
{
    [TestClass]
    public class StockServiceUnitTest
    {
        [TestMethod]
        public void TestDailyStockInfo()
        {
            IStockService stockService = new AzureStockService(new MockLogger<AzureStockService>(), new HttpRestService(new MockLogger<HttpRestService>()));
            string url = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=603189.SHH&outputsize=full&apikey=NXMKD2BQGY8WWV58";
            var result = stockService.GetDailyStockInfoAsync(new HttpClient(), url).Result;
            Assert.IsNotNull(result);
        }
    }
}
