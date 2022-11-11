using RyanTechno.AzureApps.Domain.Currency;
using RyanTechno.AzureApps.Client.ExchangeMobile.Helpers;

namespace RyanTechno.AzureApps.Client.ExchangeMobile.ViewModels
{
    public class LiveExchangePageViewModel : BindableBase, IInitialize
    {
        public const string LIVE_EXCHANGE_DATA_PARAM = "LiveExchangeList";
        public const string LIVE_EXCHANGE_PAGE_NAME = "LiveExchangePage";

        public List<LiveExchangeRate> LiveExchangeList { get; } = new();

        public LiveExchangePageViewModel()
        {

        }

        public void Initialize(INavigationParameters parameters)
        {
            if (parameters[LIVE_EXCHANGE_DATA_PARAM] is CurrencyApiStructure liveData)
            {
                LiveExchangeList.AddRange(liveData.Quotes.Select(q => new LiveExchangeRate
                {
                    Abbreviation = q.Key.Substring(3),
                    DisplayName = q.Key.Substring(3),
                    ImageUrl = ImageConversionHelper.GetCurrencyImageFileName(q.Key.Substring(3)),
                    Rate = q.Value,
                    OppositeRate = 100 / q.Value,
                }));
            }
        }
    }
}
