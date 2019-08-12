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
    public class InternalProjectActivityService : IInternalProjectActivityService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectActivityRepository _projectActivityRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;

        public InternalProjectActivityService(IDomainManagerService domainManagerService,
                                              IProjectActivityRepository projectActivityRepository,
                                              IEventBusService eventBusService,
                                              IActivityMonitorService activityMonitorService)
        {
            _domainManagerService = domainManagerService;
            _projectActivityRepository = projectActivityRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
        }

        public async Task UpdateProjectActivity(Guid organizationId, Guid projectId, string code, ProjectActivityPutRp resource)
        {
            var activity = await _projectActivityRepository.GetProjectActivityById(organizationId, projectId, code);

            if (activity == null)
            {
                await _domainManagerService.AddNotFound($"The activity with code {code} does not exists.");
                return;
            }

            activity.ActivityStatus = resource.ActivityStatus;
            activity.Log += $" > {resource.Log}";

            _projectActivityRepository.Update(activity);

            await _projectActivityRepository.SaveChanges();
        }
    }
}
