using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using DomainModels = PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Worker.Events;

namespace PipelineSpace.Application.Services
{
    public class ProjectFeatureServiceEnvironmentService : IProjectFeatureServiceEnvironmentService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly Func<DomainModels.CloudProviderService, ICPSQueryService> _cpsQueryService;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly IDataProtectorService _dataProtectorService;

        public ProjectFeatureServiceEnvironmentService(IDomainManagerService domainManagerService,
                                                      IIdentityService identityService,
                                                      IUserRepository userRepository,
                                                      Func<DomainModels.CloudProviderService, ICPSQueryService> cpsQueryService,
                                                      IEventBusService eventBusService,
                                                      IActivityMonitorService activityMonitorService,
                                                      IDataProtectorService dataProtectorService
                                                     )
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
            _cpsQueryService = cpsQueryService;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _dataProtectorService = dataProtectorService;
        }

        public async Task CreateProjectFeatureServiceEnvironmentVariables(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, Guid environmentId, ProjectFeatureServiceEnvironmentVariablePostRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            DomainModels.User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            DomainModels.PipelineRole role = user.GetRoleInProject(projectId);
            if (role != DomainModels.PipelineRole.ProjectAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to create environments variables in this project.");
                return;
            }

            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to add a new feature.");
                return;
            }

            DomainModels.ProjectFeature projectFeature = project.GetFeatureById(featureId);
            if (projectFeature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            DomainModels.ProjectFeatureService projectFeatureService = projectFeature.GetFeatureServiceById(serviceId);
            if (projectFeatureService == null)
            {
                await _domainManagerService.AddNotFound($"The feature service with id {serviceId} does not exists.");
                return;
            }

            if (projectFeatureService.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The feature pipe with id {serviceId} must be in status Active to add/modify variables.");
                return;
            }

            DomainModels.ProjectFeatureServiceEnvironment environment = projectFeatureService.GetServiceEnvironment(environmentId);
            if (environment == null)
            {
                await _domainManagerService.AddNotFound($"The environment with id {environmentId} does not exists.");
                return;
            }

            foreach (var resourceVariable in resource.Items)
            {
                if (string.IsNullOrEmpty(resourceVariable.Name) || string.IsNullOrEmpty(resourceVariable.Value))
                {
                    await _domainManagerService.AddConflict($"The environment variable name/value is mandatory.");
                    return;
                }

                var variable = environment.GetVariableByName(resourceVariable.Name);
                if (variable != null)
                {
                    environment.SetVariable(resourceVariable.Name, resourceVariable.Value);
                }
                else
                {
                    environment.AddVariable(resourceVariable.Name, resourceVariable.Value);
                }
            }

            _userRepository.Update(user);
            await _userRepository.SaveChanges();

            var @event = new ProjectFeatureEnvironmentCreatedEvent(_correlationId)
            {
                OrganizationId = organization.OrganizationId,
                OrganizationName = organization.Name,
                ProjectId = project.ProjectId,
                ProjectExternalId = project.ProjectExternalId,
                ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                ProjectName = project.Name,
                CMSType = project.OrganizationCMS.Type,
                CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken),
                ReleseStageId = projectFeatureService.ReleaseStageId.Value
            };

            @event.Environments = new List<ProjectFeatureEnvironmentItemCreatedEvent>();
            foreach (var item in projectFeatureService.Environments)
            {
                var parentEnvironment = projectFeature.GetEnvironments().First(x => x.ProjectFeatureEnvironmentId == item.ProjectFeatureEnvironmentId);

                var featureServiceEnvironment = new ProjectFeatureEnvironmentItemCreatedEvent();
                featureServiceEnvironment.Name = parentEnvironment.Name;
                featureServiceEnvironment.RequiredApproval = parentEnvironment.RequiresApproval;
                featureServiceEnvironment.Rank = parentEnvironment.Rank;

                featureServiceEnvironment.Variables = new List<ProjectFeatureEnvironmentItemVariableCreatedEvent>();
                foreach (var variable in parentEnvironment.Variables)
                {
                    featureServiceEnvironment.Variables.Add(new ProjectFeatureEnvironmentItemVariableCreatedEvent()
                    {
                        Name = variable.Name,
                        Value = variable.Value
                    });
                }
                foreach (var variable in item.Variables)
                {
                    featureServiceEnvironment.Variables.Add(new ProjectFeatureEnvironmentItemVariableCreatedEvent()
                    {
                        Name = variable.Name,
                        Value = variable.Value
                    });
                }
                @event.Environments.Add(featureServiceEnvironment);
            }

            //Cloud Provider Data
            @event.CPSType = project.OrganizationCPS.Type;
            @event.CPSAccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId);
            @event.CPSAccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName);
            @event.CPSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret);
            @event.CPSAccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion);

            await _eventBusService.Publish(queueName: "ProjectFeatureEnvironmentCreatedEvent", @event: @event);
        }
    }
}
