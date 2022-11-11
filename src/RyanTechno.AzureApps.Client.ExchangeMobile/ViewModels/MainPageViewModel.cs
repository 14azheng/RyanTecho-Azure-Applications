using RyanTechno.AzureApps.Domain.Currency;
using RyanTechno.AzureApps.Domain.Rest;
using RyanTechno.AzureApps.Client.ExchangeMobile.Helpers;
using RyanTechno.AzureApps.Client.ExchangeMobile.ViewModels.Models;
using RyanTechno.AzureApps.Services;
using RyanTechno.AzureApps.Services.REST;
using RyanTechno.AzureApps.Services.Storage;
using RyanTechno.AzureApps.Services.UI;
using System.Collections.ObjectModel;

namespace RyanTechno.AzureApps.Client.ExchangeMobile.ViewModels
{
    public class MainPageViewModel : BindableBase, IInitialize
    {
        private readonly INavigationService _navigationService;
        private readonly IRestService _restService;
        private readonly IStorageService _storageService;
        private readonly IAlertService _alertService;

        private CancellationTokenSource cancellationToken;

        public ObservableCollection<CurrencySubscriptionViewModel> SubscriptionList { get; } = new();

        public MainPageViewModel(INavigationService navigationService, IRestService restService, IStorageService storageService, IAlertService alertService)
        {
            _navigationService = navigationService;
            _restService = restService;
            _storageService = storageService;
            _alertService = alertService;

            SubscribeCommand = new DelegateCommand(ExecuteSubscribeCommand);
            ViewLiveExchangeCommand = new DelegateCommand(ExecuteViewLiveExchangeCommand);

            cancellationToken = new CancellationTokenSource();
        }

        public DelegateCommand SubscribeCommand { get; }

        public DelegateCommand ViewLiveExchangeCommand { get; }

        private async void ExecuteSubscribeCommand()
        {
            var result = await _storageService.SaveExchangeSubscriptionsAsync(SubscriptionList.Select(s => s.ToModel()), cancellationToken.Token);

            if (result.IsCompleted)
            {
                await _alertService.ShowAlertAsync("消息提醒", "订阅成功!", "OK");
            }
            /*
            var tokenTask = await _restService.GetAccessTokenAsync(new AuthenticationInfo
            {
                AcquireAccessTokenEndpoint = "https://auth.ryantechno.com/connect/token",
                ClientId = "exchange_api",
                ClientSecret = "bpzh5bq0Cx",
                Scope = "exchange",
            });

            if (tokenTask.Success)
            {
                string accessToken = tokenTask.AccessToken;

                var currencyTask = await _restService.GetResourcesAsync<CurrencyApiStructure>(new RestRequestInfo
                {
                    RequestEndpoint = $"https://exchange.ryantechno.com/api/currency?source=CNY&targets={string.Join(',', SubscriptionList.Where(s => s.Subscribed).Select(s => s.Abbreviation))}",
                    RequestHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", "Bearer " + accessToken }
                    }
                });

                if (currencyTask.Success)
                {
                    Console.WriteLine(currencyTask.Resource);
                }
            }
            */
        }

        private async void ExecuteViewLiveExchangeCommand()
        {
            var tokenTask = await _restService.GetAccessTokenAsync(new AuthenticationInfo
            {
                AcquireAccessTokenEndpoint = "https://auth.ryantechno.com/connect/token",
                ClientId = "exchange_api",
                ClientSecret = "bpzh5bq0Cx",
                Scope = "exchange",
            });

            if (tokenTask.Success)
            {
                string accessToken = tokenTask.AccessToken;

                var currencyTask = await _restService.GetResourcesAsync<CurrencyApiStructure>(new RestRequestInfo
                {
                    RequestEndpoint = $"https://exchange.ryantechno.com/api/currency?source=CNY&targets={string.Join(',', SubscriptionList.Where(s => s.Subscribed).Select(s => s.Abbreviation))}",
                    RequestHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", "Bearer " + accessToken }
                    }
                });

                if (currencyTask.Success)
                {
                    await _navigationService.NavigateAsync(LiveExchangePageViewModel.LIVE_EXCHANGE_PAGE_NAME, new NavigationParameters
                    {
                        { LiveExchangePageViewModel.LIVE_EXCHANGE_DATA_PARAM, currencyTask.Resource }
                    });
                }
            }
        }

        private void PrepareData()
        {
            SubscriptionList.Clear();
            SubscriptionList.Add(new CurrencySubscriptionViewModel
            {
                Abbreviation = "USD",
                DisplayName = "USD",
                ImageUrl = ImageConversionHelper.GetCurrencyImageFileName("USD"),
                Subscribed = false,
            });
            SubscriptionList.Add(new CurrencySubscriptionViewModel
            {
                Abbreviation = "HKD",
                DisplayName = "HKD",
                ImageUrl = ImageConversionHelper.GetCurrencyImageFileName("HKD"),
                Subscribed = false,
            });
            SubscriptionList.Add(new CurrencySubscriptionViewModel
            {
                Abbreviation = "JPY",
                DisplayName = "JPY",
                ImageUrl = ImageConversionHelper.GetCurrencyImageFileName("JPY"),
                Subscribed = false,
            });
            SubscriptionList.Add(new CurrencySubscriptionViewModel
            {
                Abbreviation = "EUR",
                DisplayName = "EUR",
                ImageUrl = ImageConversionHelper.GetCurrencyImageFileName("EUR"),
                Subscribed = false,
            });
        }

        public void Initialize(INavigationParameters parameters)
        {
            var task = Task.Run<TaskResult<List<CurrencySubscription>>>(() =>_storageService.LoadExchangeSubscriptionsAsync(cancellationToken.Token));

            if (task.Result.IsCompleted)
            {
                var subs = task.Result.Result;
                if (subs is not null && subs.Any())
                {
                    SubscriptionList.Clear();
                    foreach (var sub in subs)
                    {
                        SubscriptionList.Add(new CurrencySubscriptionViewModel(sub));
                    }
                }
                else
                {
                    PrepareData();
                }
            }
            else
            {
                PrepareData();
            }
        }
    }
}
