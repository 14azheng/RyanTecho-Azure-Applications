namespace RyanTechno.AzureApps.Domain.Configuration
{
    public record ExchangeServiceConfiguration
    {
        public string AuthenticationServerEndpoint { get; set; }

        public string AuthenticationClientId { get; set; }

        public string AuthenticationClientSecret { get; set; }

        public string AuthenticationScope { get; set; }

        public string ExchangeApiEndpoint { get; set; }

        public string SmtpEmailAccount { get; set; }
    }
}
