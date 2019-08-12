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
    public class InternalProjectServiceService : IInternalProjectServiceService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceRepository _projectServiceRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly IDataProtectorService _dataProtectorService;

        public InternalProjectServiceService(IDomainManagerService domainManagerService,
                                             IProjectServiceRepository projectServiceRepository,
                                             IEventBusService eventBusService,
                                             IActivityMonitorService activityMonitorService,
                                             IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceRepository = projectServiceRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _dataProtectorService = dataProtectorService;
        }

        public async Task ActivateProjectService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var service = await _projectServiceRepository.GetProjectServiceById(organizationId, projectId, serviceId);

            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The pipe with id {serviceId} does not exists.");
                return;
            }

            if (service.Status != EntityStatus.Preparing)
            {
                await _domainManagerService.AddConflict($"The pipe with id {serviceId} must be in status NEW to be activated.");
                return;
            }

            service.Activate();

            _projectServiceRepository.Update(service);

            await _projectServiceRepository.SaveChanges();
        }

        public async Task PatchProjectService(Guid organizationId, Guid projectId, Guid serviceId, ProjectServicePatchtRp resource)
        {
            var service = await _projectServiceRepository.GetProjectServiceById(organizationId, projectId, serviceId);

            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The pipe with id {serviceId} does not exists.");
                return;
            }

            if (resource.CommitStageId.HasValue)
            {
                service.CommitStageId = resource.CommitStageId.Value;
            }

            if (resource.ReleaseStageId.HasValue)
            {
                service.ReleaseStageId = resource.ReleaseStageId.Value;
            }

            if (resource.CommitServiceHookId.HasValue)
            {
                service.CommitServiceHookId = resource.CommitServiceHookId.Value;
            }

            if (resource.ReleaseServiceHookId.HasValue)
            {
                service.ReleaseServiceHookId = resource.ReleaseServiceHookId.Value;
            }

            if (resource.CodeServiceHookId.HasValue)
            {
                service.CodeServiceHookId = resource.CodeServiceHookId.Value;
            }

            if (resource.ReleaseStartedServiceHookId.HasValue)
            {
                service.ReleaseStartedServiceHookId = resource.ReleaseStartedServiceHookId.Value;
            }

            if (resource.ReleasePendingApprovalServiceHookId.HasValue)
            {
                service.ReleasePendingApprovalServiceHookId = resource.ReleasePendingApprovalServiceHookId.Value;
            }

            if (resource.ReleaseCompletedApprovalServiceHookId.HasValue)
            {
                service.ReleaseCompletedApprovalServiceHookId = resource.ReleaseCompletedApprovalServiceHookId.Value;
            }

            if (resource.PipelineStatus.HasValue)
            {
                service.PipelineStatus = resource.PipelineStatus.Value;
            }

            _projectServiceRepository.Update(service);

            await _projectServiceRepository.SaveChanges();

            if (resource.ReleaseStageId.HasValue)
            {
                var @event = new ProjectEnvironmentCreatedEvent(_correlationId)
                {
                    OrganizationId = service.Project.Organization.OrganizationId,
                    OrganizationName = service.Project.Organization.Name,
                    ProjectId = service.Project.ProjectId,
                    ProjectExternalId = service.Project.ProjectExternalId,
                    ProjectExternalEndpointId = service.Project.ProjectExternalEndpointId,
                    ProjectVSTSFakeName = service.Project.ProjectVSTSFakeName,
                    ProjectName = service.Project.Name,
                    CMSType = service.Project.OrganizationCMS.Type,
                    CMSAccountId = _dataProtectorService.Unprotect(service.Project.OrganizationCMS.AccountId),
                    CMSAccountName = _dataProtectorService.Unprotect(service.Project.OrganizationCMS.AccountName),
                    CMSAccessId = _dataProtectorService.Unprotect(service.Project.OrganizationCMS.AccessId),
                    CMSAccessSecret = _dataProtectorService.Unprotect(service.Project.OrganizationCMS.AccessSecret),
                    CMSAccessToken = _dataProtectorService.Unprotect(service.Project.OrganizationCMS.AccessToken),
                    ReleseStageId = resource.ReleaseStageId.Value
                };

                @event.Environments = new List<ProjectEnvironmentItemCreatedEvent>();

                foreach (var item in service.Environments)
                {
                    var parentEnvironment = service.Project.GetEnvironments().First(x => x.ProjectEnvironmentId == item.ProjectEnvironmentId);

                    var serviceEnvironment = new ProjectEnvironmentItemCreatedEvent();
                    serviceEnvironment.Id = item.ProjectEnvironmentId;
                    serviceEnvironment.Name = parentEnvironment.Name;
                    serviceEnvironment.RequiredApproval = parentEnvironment.RequiresApproval;
                    serviceEnvironment.Variables = new List<ProjectEnvironmentItemVariableCreatedEvent>();
                    serviceEnvironment.Rank = parentEnvironment.Rank;
                    serviceEnvironment.LastSuccessVersionId = item.LastSuccessVersionId;
                    serviceEnvironment.LastSuccessVersionName = item.LastSuccessVersionName;

                    if (parentEnvironment.Variables != null)
                    {
                        foreach (var variable in parentEnvironment.Variables)
                        {
                            serviceEnvironment.Variables.Add(new ProjectEnvironmentItemVariableCreatedEvent()
                            {
                                Name = variable.Name,
                                Value = variable.Value
                            });
                        }
                    }

                    if (item.Variables != null)
                    {
                        foreach (var variable in item.Variables)
                        {
                            serviceEnvironment.Variables.Add(new ProjectEnvironmentItemVariableCreatedEvent()
                            {
                                Name = variable.Name,
                                Value = variable.Value
                            });
                        }
                    }


                    @event.Environments.Add(serviceEnvironment);
                }

                //Cloud Provider Data
                @event.CPSType = service.Project.OrganizationCPS.Type;
                @event.CPSAccessId = _dataProtectorService.Unprotect(service.Project.OrganizationCPS.AccessId);
                @event.CPSAccessName = _dataProtectorService.Unprotect(service.Project.OrganizationCPS.AccessName);
                @event.CPSAccessSecret = _dataProtectorService.Unprotect(service.Project.OrganizationCPS.AccessSecret);
                @event.CPSAccessRegion = _dataProtectorService.Unprotect(service.Project.OrganizationCPS.AccessRegion);

                await _eventBusService.Publish(queueName: "ProjectEnvironmentCreatedEvent", @event: @event);
            }
        }
    }
}
