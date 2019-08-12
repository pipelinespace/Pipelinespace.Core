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
    public class InternalProjectServiceActivityService : IInternalProjectServiceActivityService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceActivityRepository _projectServiceActivityRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;

        public InternalProjectServiceActivityService(IDomainManagerService domainManagerService,
                                                     IProjectServiceActivityRepository projectServiceActivityRepository,
                                                     IEventBusService eventBusService,
                                                     IActivityMonitorService activityMonitorService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceActivityRepository = projectServiceActivityRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
        }

        public async Task UpdateProjectServiceActivity(Guid organizationId, Guid projectId, Guid serviceId, string code, ProjectServiceActivityPutRp resource)
        {
            var activity = await _projectServiceActivityRepository.GetProjectServiceActivityById(organizationId, projectId, serviceId, code);

            if (activity == null)
            {
                await _domainManagerService.AddNotFound($"The activity with code {code} does not exists.");
                return;
            }

            activity.ActivityStatus = resource.ActivityStatus;
            activity.Log += $" > {resource.Log}";

            _projectServiceActivityRepository.Update(activity);

            await _projectServiceActivityRepository.SaveChanges();
        }
    }
}
