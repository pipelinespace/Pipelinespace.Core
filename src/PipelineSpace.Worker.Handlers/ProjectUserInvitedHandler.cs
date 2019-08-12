using Microsoft.Extensions.Options;
using PipelineSpace.Domain;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Extensions;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectUserInvitedHandler : IEventHandler<ProjectUserInvitedEvent>
    {
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IEmailWorkerService _emailWorkerService;

        public ProjectUserInvitedHandler(IOptions<ApplicationOptions> applicationOptions,
                                         IOptions<VSTSServiceOptions> vstsOptions,
                                         IEmailWorkerService emailWorkerService)
        {
            _applicationOptions = applicationOptions;
            _vstsOptions = vstsOptions;
            _emailWorkerService = emailWorkerService;
        }

        public async Task Handle(ProjectUserInvitedEvent @event)
        {
            //string template = string.Empty;
            //string url = _applicationOptions.Value.Url;

            //if (@event.InvitationType == UserInvitationType.InternalUser)
            //{
            //    template = $"Please accept the invitation by clicking this link: <a href='{HtmlEncoder.Default.Encode($"{url}/Manage/Invitation")}'>link</a>";
            //}
            //else
            //{
            //    template = $"Please join to PipelineSpace to accept the invitation by clicking this link: <a href='{HtmlEncoder.Default.Encode($"{url}/Account/Join/{@event.ProjectUserInvitationId}")}'>link</a>";
            //}

            //await _emailWorkerService.SendEmailAsync(@event.UserEmail, "PipelineSpace Project Invitation", template);
        }
    }
}
