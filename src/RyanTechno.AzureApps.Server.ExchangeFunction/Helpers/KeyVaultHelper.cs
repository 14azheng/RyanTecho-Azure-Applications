using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace RyanTechno.AzureApps.Server.ExchangeFunction.Helpers
{
    public class KeyVaultHelper
    {
        public static async Task<string> GetSecret(string keyVaultUrl, string secretName)
        {
            SecretClient secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);

            return secret?.Value;
        }
    }
}
