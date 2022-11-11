using RyanTechno.AzureApps.Domain.Currency;

namespace RyanTechno.AzureApps.Services.Storage
{
    public interface IStorageService
    {
        Task<TaskResult> SaveExchangeSubscriptionsAsync(IEnumerable<CurrencySubscription> subscriptions, CancellationToken token);

        Task<TaskResult<List<CurrencySubscription>>> LoadExchangeSubscriptionsAsync(CancellationToken token);
    }
}
