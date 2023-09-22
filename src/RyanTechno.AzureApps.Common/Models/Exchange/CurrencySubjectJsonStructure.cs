using System.Text.Json.Serialization;

namespace RyanTechno.AzureApps.Common.Models.Exchange
{
    public class CurrencySubjectJsonStructure
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("currencies")]
        public Dictionary<string, string> Currencies { get; set; }
    }
}
