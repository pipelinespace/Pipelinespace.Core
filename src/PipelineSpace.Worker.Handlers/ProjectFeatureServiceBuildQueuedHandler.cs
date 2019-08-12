using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PipelineSpace.Domain;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectFeatureServiceBuildQueuedHandler : BaseHandler, IEventHandler<ProjectFeatureServiceBuildQueuedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly Func<ConfigurationManagementService, IProjectServiceHandlerService> _projectServiceHandlerService;
        readonly IPipelineSpaceManagerService _pipelineSpaceManagerService;
        readonly IHttpClientWrapperService _httpClientWrapperService;

        public ProjectFeatureServiceBuildQueuedHandler(IOptions<VSTSServiceOptions> vstsOptions,
                                                IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                                IOptions<ApplicationOptions> applicationOptions,
                                                Func<ConfigurationManagementService, IProjectServiceHandlerService> projectServiceHandlerService,
                                                IPipelineSpaceManagerService pipelineSpaceManagerService,
                                                IHttpClientWrapperService httpClientWrapperService,
                                                IRealtimeService realtimeService) : base(httpClientWrapperService, applicationOptions, realtimeService)
        {
            _vstsOptions = vstsOptions;
            _applicationOptions = applicationOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _projectServiceHandlerService = projectServiceHandlerService;
            _pipelineSpaceManagerService = pipelineSpaceManagerService;
            _httpClientWrapperService = httpClientWrapperService;
        }

        public async Task Handle(ProjectFeatureServiceBuildQueuedEvent @event)
        {
            string eventUrl = $"{_applicationOptions.Value.Url}/publicapi/organizations/{@event.OrganizationId}/projects/{@event.ProjectId}/features/{@event.FeatureId}/services/{@event.ServiceId}/events";
            await _httpClientWrapperService.PostAsync(eventUrl, new
            {
                SubscriptionId = Guid.NewGuid(),
                NotificationId = 1,
                Id = string.Empty,
                EventType = "git.push",
                PublisherId = "ps",
                Message = new
                {
                    Text = "Build requested from PipelineSpace"
                },
                DetailedMessage = new { },
                Resource = new { },
                Status = "Queued",
                Date = DateTime.UtcNow
            }, InternalAuthCredentials);
        }
    }
}
