using Azure;
using RyanTechno.AzureApps.Domain.Configuration;

namespace RyanTechno.AzureApps.Services.Models
{
    internal class ServiceConfigurationAzureTable : AzureTable
    {
        public const string AUTHENTICATION_CENTER_ENDPOINT_ROWKEY = "AuthCenterEndpoint";
        public const string AUTHENTICATION_CLIENT_ID_ROWKEY = "AuthClientId";
        public const string AUTHENTICATION_CLIENT_SECRET_ROWKEY = "AuthClientSecret";
        public const string AUTHENTICATION_SCOPE_ROWKEY = "AuthScope";
        public const string EXCHANGE_API_ENDPOINT_ROWKEY = "ExchangeApiEndpoint";
        public const string SMTP_EMAIL_ACCOUNT_ROWKEY = "SmtpEmailAccount";

        public string Value { get; set; }
    }

    internal static class ServiceConfigurationAuzreTableExtension
    {
        public static ExchangeServiceConfiguration ToModel(this Pageable<ServiceConfigurationAzureTable> tableQuery)
        {
            if (tableQuery is null)
            {
                return null;
            }

            ExchangeServiceConfiguration config = new();

            foreach (ServiceConfigurationAzureTable row in tableQuery)
            {
                switch (row.RowKey)
                {
                    case ServiceConfigurationAzureTable.AUTHENTICATION_CENTER_ENDPOINT_ROWKEY:
                        config.AuthenticationServerEndpoint = row.Value;
                        break;
                    case ServiceConfigurationAzureTable.AUTHENTICATION_CLIENT_ID_ROWKEY:
                        config.AuthenticationClientId = row.Value;
                        break;
                    case ServiceConfigurationAzureTable.AUTHENTICATION_CLIENT_SECRET_ROWKEY:
                        config.AuthenticationClientSecret = row.Value;
                        break;
                    case ServiceConfigurationAzureTable.AUTHENTICATION_SCOPE_ROWKEY:
                        config.AuthenticationScope = row.Value;
                        break;
                    case ServiceConfigurationAzureTable.EXCHANGE_API_ENDPOINT_ROWKEY:
                        config.ExchangeApiEndpoint = row.Value;
                        break;
                    case ServiceConfigurationAzureTable.SMTP_EMAIL_ACCOUNT_ROWKEY:
                        config.SmtpEmailAccount = row.Value;
                        break;
                }
            }
            return config;
        }
    }
}
