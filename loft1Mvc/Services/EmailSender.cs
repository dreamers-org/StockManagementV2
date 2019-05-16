using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace StockManagement.Services
{
    public class EmailSender : IEmailSender
    {
        //string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        string apiKey = "";

        public Task SendEmailAsync(string email, string subject, string message) => Execute(apiKey, subject, message, email);

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            try
            {
                var client = new SendGridClient(apiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("Info@loft1.it", "Loft1"),
                    Subject = subject,
                    PlainTextContent = message,
                    HtmlContent = message
                };
                msg.AddTo(new EmailAddress(email));

                msg.SetClickTracking(false, false);

                return client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }
    }
}
