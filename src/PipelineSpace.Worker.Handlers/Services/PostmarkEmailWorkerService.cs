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
    public class PostmarkEmailWorkerService : IEmailWorkerService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await Task.Run(() => {
                Console.WriteLine($"Send email to {email} with subject {subject} and body {message}");
            });
        }
    }
}
