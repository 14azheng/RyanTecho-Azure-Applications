namespace RyanTechno.AzureApps.Common.Models.Infrastructure
{
    public class EmailOptions
    {
        public string Subject { get; init; }

        public string Body { get; init; }

        public string FromAddress { get; init; }

        public IList<string> ToAddresses { get; init; } = new List<string>();

        public IList<string> CcAddresses { get; init; } = new List<string>();
    }
}
