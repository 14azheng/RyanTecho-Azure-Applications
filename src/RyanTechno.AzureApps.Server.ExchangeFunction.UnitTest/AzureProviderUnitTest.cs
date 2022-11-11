using RyanTechno.AzureApps.Server.ExchangeFunction.Helpers;

namespace RyanTechno.AzureApps.Server.ExchangeFunction.UnitTest
{
    [TestClass]
    public class AzureProviderUnitTest
    {
        [TestMethod]
        public void TestGetSubscribedCurrencyList()
        {
            AzureStorageInfo storageInfo = new AzureStorageInfo
            {
                ConnectionString = "DefaultEndpointsProtocol=https;AccountName=ryantechnostorage;AccountKey=y4z2N/6dWXUoPvBBphk25rZ6KX9AgYqxfoHIoTEYTmKTzQRpQ6jvggHINDPf+eQae8KLcEVZ/TJD+ASt+yaGWg==;EndpointSuffix=core.windows.net",
                Endpoint = "https://ryantechnostorage.table.core.windows.net/",
                AccountName = "ryantechnostorage",
                AccountKey = "y4z2N/6dWXUoPvBBphk25rZ6KX9AgYqxfoHIoTEYTmKTzQRpQ6jvggHINDPf+eQae8KLcEVZ/TJD+ASt+yaGWg==",
            };

            var currentList = AzureTableHelper.GetSubscribedCurrencyList(storageInfo);
            string targetList = string.Join(',', currentList.Select(c => c.Target));

            Assert.IsNotNull(currentList);
            Assert.IsTrue(currentList.Count > 0);
            Assert.IsNotNull(targetList);
        }

        [TestMethod]
        public async Task TestGetSecret()
        {
            string secret = await KeyVaultHelper.GetSecret("https://exchange-service-vault.vault.azure.net/", "storage-account-key");

            Assert.IsNotNull(secret);
        }
    }
}
