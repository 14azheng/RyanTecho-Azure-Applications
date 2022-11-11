using Microsoft.Extensions.Logging;
using RyanTechno.AzureApps.Server.ExchangeFunction.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace RyanTechno.AzureApps.Server.ExchangeFunction.Helpers
{
    public class OutlookEmailProvider
    {
        private string _emailAccount;
        private string _emailPassword;
        private readonly ILogger _logger;

        public OutlookEmailProvider(string emailAccount, string emailPassword, ILogger log)
        {
            _emailAccount = emailAccount;
            _emailPassword = emailPassword;
            _logger = log;
        }

        public bool SendTestingEmail()
        {
            return SendEmail(new OutlookEmailOptions
            {
                Body = "Body",
                Subject = "Ryan Techno Testing Email",
                FromAddress = _emailAccount,
                ToAddresses = new List<string>() { _emailAccount },
            });
        }

        public bool SendExchangeRateNotificationEmail(IImmutableList<CurrencyAzureTable> exceedCurrencyList, IImmutableList<CurrencyAzureTable> deficientCurrencyList)
        {
            return SendEmail(new OutlookEmailOptions
            {
                Body = $"<html><body>赶快去买/卖外汇啦 <br/><br/><b>马上入手</b><br/><br/>{string.Join("<br/>", deficientCurrencyList.Select(d => $"{d.Target} - 设定值: {d.MinLevel} / 当前值: {d.LiveQuote}"))}" +
                        $"<br/><br/><b>马上出手</b><br/><br/>{string.Join("<br/>", exceedCurrencyList.Select(e => $"{e.Target} - 设定值: {e.MaxLevel} / 当前值: {e.LiveQuote.ToString("0.000")}"))}</body></html>",
                Subject = $"汇率变动 - {string.Join(" | ", exceedCurrencyList.Select(e => e.Target).Union(deficientCurrencyList.Select(d => d.Target)))}",
                FromAddress = _emailAccount,
                ToAddresses = new List<string>() { _emailAccount },
            });
        }

        private bool SendEmail(OutlookEmailOptions options)
        {
            NetworkCredential basicCredential = new NetworkCredential(this._emailAccount, this._emailPassword);
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(options.FromAddress);

            foreach(string to in options.ToAddresses)
            {
                msg.To.Add(to);
            }

            foreach (string cc in options.CcAddresses)
            {
                msg.CC.Add(cc);
            }

            msg.Subject = options.Subject;
            msg.IsBodyHtml = true;
            msg.Body = options.Body;

            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp-mail.outlook.com";
            smtp.Credentials = basicCredential;
            smtp.EnableSsl = true;

            _logger.LogInformation($"Start sending email...Message: {msg} | To: {msg.To[0].Address}");

            try
            {
                smtp.Send(msg);

                _logger.LogInformation("End sending email...");

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Outlook Email Sending Error");
                return false;
            }
            finally
            {
                smtp.Dispose();
            }
        }
    }

    internal record OutlookEmailOptions
    {
        public string Subject { get; init; }

        public string Body { get; init; }

        public string FromAddress { get; init; }

        public IList<string> ToAddresses { get; init; } = new List<string>();

        public IList<string> CcAddresses { get; init; } = new List<string>();
    }
}
