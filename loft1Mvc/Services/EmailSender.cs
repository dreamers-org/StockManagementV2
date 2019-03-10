using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Services
{
	public class EmailSender : IEmailSender
	{
		string apiKey = "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";
		public EmailSender()
		{
		}

		public Task SendEmailAsync(string email, string subject, string message)
		{
			return Execute(apiKey, subject, message, email);
		}

		public Task Execute(string apiKey, string subject, string message, string email)
		{
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage()
			{
				From = new EmailAddress("luca.bellavia.dev@gmail.com", "Luca Bellavia"),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message
			};
			msg.AddTo(new EmailAddress(email));

			msg.SetClickTracking(false, false);

			return client.SendEmailAsync(msg);
		}
	}
}
