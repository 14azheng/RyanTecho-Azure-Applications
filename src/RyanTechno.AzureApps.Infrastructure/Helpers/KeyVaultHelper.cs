using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace RyanTechno.AzureApps.Infrastructure.Helpers
{
    public class KeyVaultHelper
    {
        public static async Task<string> GetSecretAsync(string keyVaultUrl, string secretName)
        {
            SecretClient secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);

            return secret?.Value;
        }
    }
}
