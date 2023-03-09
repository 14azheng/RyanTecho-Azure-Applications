﻿using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Domain.Configuration;
using RyanTechno.AzureApps.Domain.Exchange;
using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Common.Interfaces.Exchange
{
    public interface IExchangeService
    {
        ExchangeServiceConfiguration GetExchangeServiceConfiguration(AzureStorageInfo storageInfo);

        IImmutableList<CurrencySubscription> GetSubscribedCurrencyList(AzureStorageInfo storageInfo);

        Task<ServiceResult> SaveExchangeSubscriptionsAsync(IEnumerable<CurrencySubscription> subscriptions, CancellationToken token);

        Task<ServiceResult<List<CurrencySubscription>>> LoadExchangeSubscriptionsAsync(CancellationToken token);
    }
}
