using RyanTechno.AzureApps.Common.Interfaces.Infrastructure;
using RyanTechno.AzureApps.Common.Models.Infrastructure;
using RyanTechno.AzureApps.Domain.Exchange;
using RyanTechno.AzureApps.Infrastructure.Helpers;
using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Services.Infrastructure
{
    public class OutlookEmailService : IEmailService
    {
        private EmailSenderCredential _senderCredential;

        public OutlookEmailService()
        {
        }

        public bool SendExchangeRateNotificationEmail(IImmutableList<CurrencySubscription> exceedCurrencyList, IImmutableList<CurrencySubscription> deficientCurrencyList)
        {
            if (_senderCredential is null)
                throw new ArgumentNullException("Sender credential is not set.");

            if (_senderCredential is not OutlookSenderCredential)
                throw new ArgumentException($"Provided email sender credential is not a {typeof(OutlookSenderCredential)}.");

            return OutlookEmailHelper.SendEmail(_senderCredential as OutlookSenderCredential, new EmailOptions
            {
                Body = $"<html><body>赶快去买/卖外汇啦 <br/><br/><b>马上入手</b><br/><br/>{string.Join("<br/>", deficientCurrencyList.Select(d => $"{d.Target} - 设定值: {d.MinMonitorRate} / 当前值: {d.LiveQuote}"))}" +
                        $"<br/><br/><b>马上出手</b><br/><br/>{string.Join("<br/>", exceedCurrencyList.Select(e => $"{e.Target} - 设定值: {e.MaxMonitorRate} / 当前值: {e.LiveQuote.Value.ToString("0.000")}"))}</body></html>",
                Subject = $"汇率变动 - {string.Join(" | ", exceedCurrencyList.Select(e => e.Target).Union(deficientCurrencyList.Select(d => d.Target)))}",
                FromAddress = "zheng14a@hotmail.com",
                ToAddresses = new List<string>() { "zheng14a@hotmail.com" },
            });
        }

        public bool SendTestingEmail()
        {
            if (_senderCredential is null)
                throw new ArgumentNullException("Sender credential is not set.");

            if (_senderCredential is not OutlookSenderCredential)
                throw new ArgumentException($"Provided email sender credential is not a {typeof(OutlookSenderCredential)}.");

            return OutlookEmailHelper.SendEmail(_senderCredential as OutlookSenderCredential, new EmailOptions
            {
                Body = "Body",
                Subject = "Ryan Techno Testing Email",
                FromAddress = "zheng14a@hotmail.com",
                ToAddresses = new List<string>() { "zheng14a@hotmail.com" },
            });
        }

        public void SetSenderCredential(EmailSenderCredential credential)
        {
            _senderCredential = credential;
        }
    }
}
