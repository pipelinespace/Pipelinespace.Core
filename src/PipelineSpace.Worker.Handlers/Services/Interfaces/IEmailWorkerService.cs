using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services.Interfaces
{
    public interface IEmailWorkerService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
