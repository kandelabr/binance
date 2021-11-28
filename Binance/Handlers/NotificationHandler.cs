using Binance.API.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Binance.Handlers
{
    /// <summary>
    /// Notification handler class
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ILogger<NotificationHandler> _logger;

        public NotificationHandler(IOptions<EmailConfiguration> emailConfiguration, ILogger<NotificationHandler> logger)
        {
            _emailConfiguration = emailConfiguration.Value;
            _logger = logger;
        }

        public async Task SendEmail(string subject, string message)
        {
            try
            {
                if (!bool.Parse(_emailConfiguration.Enabled))
                    return;

                using (MailMessage mm = new MailMessage(_emailConfiguration.From, _emailConfiguration.To))
                {
                    mm.Subject = subject;
                    mm.Body = message;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(_emailConfiguration.User, _emailConfiguration.Pass);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while sending email with subject {subject} and message {message}");
            }
        }
    }
}
