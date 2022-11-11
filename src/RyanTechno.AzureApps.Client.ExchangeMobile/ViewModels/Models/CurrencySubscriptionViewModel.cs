using RyanTechno.AzureApps.Domain.Currency;

namespace RyanTechno.AzureApps.Client.ExchangeMobile.ViewModels.Models
{
    public class CurrencySubscriptionViewModel : BindableBase
    {
        public string Abbreviation { get; init; }

        public string DisplayName { get; init; }

        private bool subscribed = false;
        public bool Subscribed
        {
            get => this.subscribed;
            set => SetProperty(ref this.subscribed, value);
        }

        public decimal? MinMonitorRate { get; set; }

        public decimal? MaxMonitorRate { get; set; }

        public string ImageUrl { get; init; }

        public CurrencySubscriptionViewModel()
        {

        }

        public CurrencySubscriptionViewModel(CurrencySubscription currencySubscription)
        {
            Abbreviation = currencySubscription.Abbreviation;
            DisplayName = currencySubscription.DisplayName;
            Subscribed = currencySubscription.Subscribed;
            MinMonitorRate = currencySubscription.MinMonitorRate;
            MaxMonitorRate = currencySubscription.MaxMonitorRate;
            ImageUrl = currencySubscription.ImageUrl;
        }

        public CurrencySubscription ToModel() => new CurrencySubscription
        {
            Abbreviation = Abbreviation,
            DisplayName = DisplayName,
            Subscribed = Subscribed,
            MinMonitorRate = MinMonitorRate,
            MaxMonitorRate = MaxMonitorRate,
            ImageUrl = ImageUrl,
        };
    }
}
