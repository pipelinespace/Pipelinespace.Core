using Microsoft.Extensions.Options;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers
{
    public class ProjectFeatureCompletedHandler : IEventHandler<ProjectFeatureCompletedEvent>
    {
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IOptions<ApplicationOptions> _applicationOptions;
        readonly Func<ConfigurationManagementService, IProjectFeatureHandlerService> _projectFeatureHandlerService;

        public ProjectFeatureCompletedHandler(IOptions<VSTSServiceOptions> vstsOptions,
                                             IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                             IOptions<ApplicationOptions> applicationOptions,
                                             Func<ConfigurationManagementService, IProjectFeatureHandlerService> projectFeatureHandlerService)
        {
            _vstsOptions = vstsOptions;
            _applicationOptions = applicationOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _projectFeatureHandlerService = projectFeatureHandlerService;
        }

        public async Task Handle(ProjectFeatureCompletedEvent @event)
        {
            await _projectFeatureHandlerService(@event.CMSType).CompleteProjectFeature(@event);
        }
    }
}
