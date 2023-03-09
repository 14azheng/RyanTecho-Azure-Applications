namespace RyanTechno.AzureApps.Common.Models
{
    public record AzureStorageInfo
    {
        public string ConnectionString { get; init; }

        public string Endpoint { get; init; }

        public string AccountName { get; init; }

        public string AccountKey { get; init; }
    }
}
