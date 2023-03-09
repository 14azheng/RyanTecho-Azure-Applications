using RyanTechno.AzureApps.Common.Models.Exchange;
using RyanTechno.AzureApps.Infrastructure.Helpers;

namespace RyanTechno.AzureApps.Infrastructure.UnitTest
{
    [TestClass]
    public class HelperUnitTest
    {
        [TestMethod]
        public void TestIOHelper()
        {
            string jsonFile = @"D:\Personal\Github\Azure Apps\src\RyanTechno.AzureApps.Server.ExchangeApi\Resources\Exchange\sample\boardcast-sample.json";

            DailyExchangeRateBoardcastApiStructure boardcast = IOHelper.DeserializeJsonFile<DailyExchangeRateBoardcastApiStructure>(jsonFile);

            Assert.IsNotNull(boardcast);
        }
    }
}