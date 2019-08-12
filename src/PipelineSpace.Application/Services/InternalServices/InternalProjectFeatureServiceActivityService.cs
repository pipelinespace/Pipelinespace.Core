using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.InternalServices.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Worker.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services.InternalServices
{
    public class InternalProjectFeatureServiceActivityService : IInternalProjectFeatureServiceActivityService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectFeatureServiceActivityRepository _projectFeatureServiceActivityRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;

        public InternalProjectFeatureServiceActivityService(IDomainManagerService domainManagerService,
                                                            IProjectFeatureServiceActivityRepository projectFeatureServiceActivityRepository,
                                                            IEventBusService eventBusService,
                                                            IActivityMonitorService activityMonitorService)
        {
            _domainManagerService = domainManagerService;
            _projectFeatureServiceActivityRepository = projectFeatureServiceActivityRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
        }

        public async Task UpdateProjectFeatureServiceActivity(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, string code, ProjectFeatureServiceActivityPutRp resource)
        {
            var activity = await _projectFeatureServiceActivityRepository.GetProjectFeatureServiceActivityById(organizationId, projectId, featureId, serviceId, code);

            if (activity == null)
            {
                await _domainManagerService.AddNotFound($"The activity with code {code} does not exists.");
                return;
            }

            activity.ActivityStatus = resource.ActivityStatus;
            activity.Log += $" > {resource.Log}";

            _projectFeatureServiceActivityRepository.Update(activity);

            await _projectFeatureServiceActivityRepository.SaveChanges();
        }
    }
}
