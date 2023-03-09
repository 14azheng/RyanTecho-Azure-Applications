using RyanTechno.AzureApps.Common.Models.Infrastructure;
using System.Net;
using System.Net.Mail;

namespace RyanTechno.AzureApps.Infrastructure.Helpers
{
    public class OutlookEmailHelper
    {
        public static bool SendEmail(OutlookSenderCredential senderCredential, EmailOptions options)
        {
            NetworkCredential basicCredential = new NetworkCredential(senderCredential.AccountName, senderCredential.Password);
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

            //logger.LogInformation($"Start sending email...Message: {msg} | To: {msg.To[0].Address}");

            try
            {
                smtp.Send(msg);

                //logger.LogInformation("End sending email...");

                return true;
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "Outlook Email Sending Error");
                return false;
            }
            finally
            {
                smtp.Dispose();
            }
        }
    }

    
}
