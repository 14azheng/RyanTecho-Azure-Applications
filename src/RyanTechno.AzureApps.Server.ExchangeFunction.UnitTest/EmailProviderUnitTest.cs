using RyanTechno.AzureApps.Server.ExchangeFunction.Helpers;
using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Server.ExchangeFunction.UnitTest
{
    [TestClass]
    public class EmailProviderUnitTest
    {
        [TestMethod]
        public void TestOutlookNotificationEmail()
        {
            Models.CurrencyAzureTable[] exceedCurrencies = new Models.CurrencyAzureTable[]
            {
                new()
                {
                    Source = "CNY",
                    Target = "JPY",
                    MinLevel = 4.5d,
                    MaxLevel = 5.5d,
                    LiveQuote = 5.6M,
                },
            };

            Models.CurrencyAzureTable[] deficientCurrencies = new Models.CurrencyAzureTable[]
            {
                new()
                {
                    Source = "CNY",
                    Target = "USD",
                    MinLevel = 6.0d,
                    MaxLevel = 7.3d,
                    LiveQuote = 5.9M,
                },
            };

            OutlookEmailProvider provider = new OutlookEmailProvider("zheng14a@hotmail.com", "Sierra@123", null);
            var sent = provider.SendExchangeRateNotificationEmail(exceedCurrencies.ToImmutableArray(), deficientCurrencies.ToImmutableArray());

            Assert.IsTrue(sent);
        }

        [TestMethod]
        public void TestOutlookTestingEmail()
        {
            OutlookEmailProvider provider = new OutlookEmailProvider("zheng14a@hotmail.com", "Sierra@123", null);
            var sent = provider.SendTestingEmail();

            Assert.IsTrue(sent);
        }
    }
}