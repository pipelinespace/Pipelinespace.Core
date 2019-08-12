using PipelineSpace.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Notification.Postmark
{
    public class PostmarkEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await Task.Run(() => {
                Console.WriteLine($"Send email to {email} with subject {subject} and body {message}");
            });
        }
    }
}
