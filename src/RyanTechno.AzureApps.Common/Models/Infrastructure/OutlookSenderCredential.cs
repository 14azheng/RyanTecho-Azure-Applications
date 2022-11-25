using RyanTechno.AzureApps.Common.Models.Infrastructure;

namespace RyanTechno.AzureApps.Common.Models.Infrastructure
{
    public class OutlookSenderCredential : EmailSenderCredential
    {
        public string AccountName { get; init; }

        public string Password { get; init; }
    }
}
