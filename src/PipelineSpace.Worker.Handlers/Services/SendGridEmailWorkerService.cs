using Microsoft.Extensions.Options;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class SendGridEmailWorkerService : IEmailWorkerService
    {
        private readonly SendGridOptions _sendGridOptions;
        public SendGridEmailWorkerService(IOptions<NotificationOptions> notificationOptions)
        {
            this._sendGridOptions = notificationOptions.Value.SendGrid;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
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
                throw new Exception($"Sendgrid exeption. Response Code ${response.StatusCode}");
            }
        }
    }
}
