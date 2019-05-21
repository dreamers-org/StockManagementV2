using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace StockManagement.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message) => Execute(subject, message, email);

        public Task Execute(string subject, string message, string email)
        {
            try
            {
                string key = _configuration.GetValue<string>("SendGridApiKey");

                if (string.IsNullOrEmpty(key))
                {
                    key = Environment.GetEnvironmentVariable("SENDGRID_API_KEY", EnvironmentVariableTarget.User);
                }

                Utility.CheckNull(key);

                var client = new SendGridClient(key);

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
