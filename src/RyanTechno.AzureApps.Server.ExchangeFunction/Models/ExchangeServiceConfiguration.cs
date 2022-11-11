using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyanTechno.AzureApps.Server.ExchangeFunction.Models
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
