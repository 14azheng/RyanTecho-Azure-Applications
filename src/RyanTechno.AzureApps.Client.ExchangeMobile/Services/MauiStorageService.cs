using RyanTechno.AzureApps.Domain.Currency;
using RyanTechno.AzureApps.Services;
using RyanTechno.AzureApps.Services.Storage;

namespace RyanTechno.AzureApps.Client.ExchangeMobile.Services
{
    public class MauiStorageService : IStorageService
    {
        private const string SUBS_SUFFIX = "_SUBS";
        private const string MIN_SUFFIX = "_SUBS_MIN";
        private const string MAX_SUFFIX = "_SUBS_MAX";
        private const string TOTAL_SUBS_CUR_LIST = "TOTAL_SUBS_CUR_LIST";

        public MauiStorageService()
        {

        }

        public async Task<TaskResult<List<CurrencySubscription>>> LoadExchangeSubscriptionsAsync(CancellationToken token)
        {
            try
            {
                List<CurrencySubscription> subscriptions = new List<CurrencySubscription>();
                await Task.Run(() =>
                {
                    if (Preferences.ContainsKey(TOTAL_SUBS_CUR_LIST))
                    {
                        string[] currencies = Preferences.Get(TOTAL_SUBS_CUR_LIST, string.Empty)!.Split(',');

                        subscriptions.AddRange(currencies.Select(c => new CurrencySubscription
                        {
                            Abbreviation = c,
                            Subscribed = Preferences.Get(c + SUBS_SUFFIX, false),
                            MinMonitorRate = Convert.ToDecimal(Preferences.Get(c + MIN_SUFFIX, 0d)),
                            MaxMonitorRate = Convert.ToDecimal(Preferences.Get(c + MAX_SUFFIX, 0d)),
                        }));
                    }
                }, token);

                return new TaskResult<List<CurrencySubscription>>
                {
                    IsCompleted = true,
                    Result = subscriptions,
                };
            }
            catch (Exception ex)
            {
                return new TaskResult<List<CurrencySubscription>> { IsCompleted = false, Error = ex.Message };
            }
        }

        public async Task<TaskResult> SaveExchangeSubscriptionsAsync(IEnumerable<CurrencySubscription> subscriptions, CancellationToken token)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (subscriptions is not null && subscriptions.Any())
                    {
                        foreach (CurrencySubscription subscription in subscriptions)
                        {
                            Preferences.Set(subscription.Abbreviation + SUBS_SUFFIX, subscription.Subscribed);

                            if (subscription.MinMonitorRate.HasValue)
                                Preferences.Set(subscription.Abbreviation + MIN_SUFFIX, (double)subscription.MinMonitorRate.Value);

                            if (subscription.MaxMonitorRate.HasValue)
                                Preferences.Set(subscription.Abbreviation + MAX_SUFFIX, (double)subscription.MaxMonitorRate.Value);
                        }

                        Preferences.Set(TOTAL_SUBS_CUR_LIST, string.Join(',', subscriptions.Select(s => s.Abbreviation)));
                    }
                }, token);

                return TaskResult.Success;
            }
            catch (Exception ex)
            {
                return new TaskResult { IsCompleted = false, Error = ex.Message };
            }

        }
    }
}
