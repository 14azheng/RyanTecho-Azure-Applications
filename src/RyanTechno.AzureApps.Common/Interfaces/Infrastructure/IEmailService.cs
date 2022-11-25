using RyanTechno.AzureApps.Common.Models.Infrastructure;
using RyanTechno.AzureApps.Domain.Exchange;
using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Common.Interfaces.Infrastructure
{
    public interface IEmailService
    {
        void SetSenderCredential(EmailSenderCredential credential);

        bool SendTestingEmail();

        bool SendExchangeRateNotificationEmail(IImmutableList<CurrencySubscription> exceedCurrencyList, IImmutableList<CurrencySubscription> deficientCurrencyList);
    }
}
