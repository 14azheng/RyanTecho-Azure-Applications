using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace RyanTechno.AzureApps.Infrastructure.Helpers
{
    public class KeyVaultHelper
    {
        public static async Task<string> GetSecretAsync(string keyVaultUrl, string secretName)
        {
            // Use default azure credential to access Key Vault.
            // For development, see https://www.rahulpnath.com/blog/defaultazurecredential-from-azure-sdk/ for local dev setup.
            // Cannot use Azure subscription account, must use a new user account created in AAD and assign suffient permission to access Key Vault.
            SecretClient secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);

            return secret?.Value;
        }

        private static DefaultAzureCredential ConstructDevAzureCredential(string tenantId)
        {
            DefaultAzureCredentialOptions default_azure_credential_options;

            DefaultAzureCredential azure_credential_default;


            // Exclude all to begin with ...

            default_azure_credential_options = new DefaultAzureCredentialOptions
            {

                ExcludeAzureCliCredential = true,
                ExcludeAzurePowerShellCredential = true,
                ExcludeEnvironmentCredential = true,
                ExcludeInteractiveBrowserCredential = true,
                ExcludeManagedIdentityCredential = true,
                ExcludeSharedTokenCacheCredential = true,
                ExcludeVisualStudioCodeCredential = true,
                ExcludeVisualStudioCredential = true

            };


            // Try to use the Visual Studio credential ...

            default_azure_credential_options.ExcludeVisualStudioCredential = false;


            /*
            The tenant ID of the user to authenticate.  The default is null and will authenticate users to 
            their default tenant. The value can also be set by setting the environment variable AZURE_TENANT_ID.
            Here we set the value explicitly.  The value was obtained AFTER logging into Azure via the CLI, i.e.:
             > az login
             > az account list
               [
                {
                 ...
                 "tenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
                 ...
                }
               ]
            */

            default_azure_credential_options.VisualStudioTenantId = tenantId;

            // Create credentials and add Azure KeyVault config keys / values ...

            azure_credential_default = new DefaultAzureCredential(default_azure_credential_options);

            return azure_credential_default;
        }
    }
}
