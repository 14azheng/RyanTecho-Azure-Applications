using RyanTechno.AzureApps.Common.Models.Exchange;
using RyanTechno.AzureApps.Infrastructure.Helpers;

namespace RyanTechno.AzureApps.Services.UnitTest
{
    [TestClass]
    public class ExchangeServiceUnitTest
    {
        [TestMethod]
        public void TestSummaryExchangeRate()
        {
            string jsonFile = @"D:\Personal\Github\Azure Apps\src\RyanTechno.AzureApps.Server.ExchangeApi\Resources\Exchange\sample\boardcast-sample.json";

            DailyExchangeRateBoardcastApiStructure boardcast = IOHelper.DeserializeJsonFile<DailyExchangeRateBoardcastApiStructure>(jsonFile);
            string body = boardcast.ToEmailBody();

            Assert.IsNotNull(body);
        }
    }
}
