using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RyanTechno.AzureApps.Common.Interfaces.Exchange;
using RyanTechno.AzureApps.Common.Interfaces.Infrastructure;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Services.Network;
using RyanTechno.AzureApps.Services.Exchange;
using RyanTechno.AzureApps.Services.Infrastructure;

[assembly: FunctionsStartup(typeof(RyanTechno.AzureApps.Server.ExchangeFunction.Startup))]

namespace RyanTechno.AzureApps.Server.ExchangeFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();

            builder.Services.AddSingleton<IHttpRestService, HttpRestService>();
            builder.Services.AddSingleton<IEmailService, OutlookEmailService>();
            builder.Services.AddSingleton<IExchangeService, AzureExchangeService>();

            //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
        }
    }
}
