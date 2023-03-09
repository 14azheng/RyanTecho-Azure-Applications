using Azure;
using Azure.Data.Tables;

namespace RyanTechno.AzureApps.Services.Models
{
    internal class AzureTable : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }
    }
}
