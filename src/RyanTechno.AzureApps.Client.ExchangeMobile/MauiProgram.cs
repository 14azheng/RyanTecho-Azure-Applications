using Prism.DryIoc;
using RyanTechno.AzureApps.Client.ExchangeMobile.ViewModels;
using RyanTechno.AzureApps.Client.ExchangeMobile.Views;
using RyanTechno.AzureApps.Services.REST;
using RyanTechno.AzureApps.Services.Storage;
using RyanTechno.AzureApps.Client.ExchangeMobile.Services;
using RyanTechno.AzureApps.Services.UI;

namespace RyanTechno.AzureApps.Client.ExchangeMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UsePrism(new DryIocContainerExtension(), prism =>
                {
                    prism.RegisterTypes(containerRegistry =>
                    {
                        containerRegistry.RegisterForNavigation<MainPage>();
                        containerRegistry.RegisterForNavigation<LiveExchangePage>(LiveExchangePageViewModel.LIVE_EXCHANGE_PAGE_NAME);

                        containerRegistry.Register<IRestService, RestService>();
                        containerRegistry.Register<IStorageService, MauiStorageService>();
                        containerRegistry.Register<IAlertService, MauiAlertService>();
                    })
                    .OnAppStart("/MainPage");
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            return builder.Build();
        }
    }
}