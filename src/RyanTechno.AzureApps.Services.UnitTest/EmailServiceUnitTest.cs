using RyanTechno.AzureApps.Common.Interfaces.Infrastructure;
using RyanTechno.AzureApps.Domain.Exchange;
using RyanTechno.AzureApps.Services.Infrastructure;
using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Services.UnitTest
{
    [TestClass]
    public class EmailServiceUnitTest
    {
        [TestMethod]
        public void TestOutlookEmailService()
        {
            IImmutableList<CurrencySubscription> exceedCurrencies = ImmutableArray.Create<CurrencySubscription>(new CurrencySubscription
            {
                Source = "CNY",
                Target = "GBP",
                MaxMonitorRate = 8,
                MinMonitorRate = 7,
                LiveQuote = 8.62857m,
            });

            IImmutableList<CurrencySubscription> deficientCurrencies = ImmutableArray<CurrencySubscription>.Empty;

            IEmailService service = new OutlookEmailService();
            var result = service.SendExchangeRateNotificationEmail(exceedCurrencies, deficientCurrencies);

            Assert.IsTrue(result);
        }
    }
}