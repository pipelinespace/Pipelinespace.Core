using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Infra.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Notification.SendGrid
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly ILogger<SendGridEmailSender> _logger;
        private readonly SendGridOptions _sendGridOptions;

        public SendGridEmailSender(ILogger<SendGridEmailSender> logger, IOptions<NotificationOptions> notificationOptions)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._sendGridOptions = notificationOptions.Value.SendGrid;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var apiKey = this._sendGridOptions.ApiKey;
                var from = this._sendGridOptions.From;

                var client = new SendGridClient(apiKey);
                var formEmailAddress = new EmailAddress(from);
                var toEmailAddress = new EmailAddress(email);

                var msg = MailHelper.CreateSingleEmail(formEmailAddress, toEmailAddress, subject, message, message);
                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    this._logger.LogError(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.ToString());
            }
        }
    }
}
