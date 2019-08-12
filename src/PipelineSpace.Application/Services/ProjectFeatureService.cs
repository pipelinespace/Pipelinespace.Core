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
using PipelineSpace.Worker.Events;
using System.Linq;
using PipelineSpace.Domain.Models;

namespace PipelineSpace.Application.Services
{
    public class ProjectFeatureService : IProjectFeatureService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly IDataProtectorService _dataProtectorService;
        readonly IProjectCloudCredentialService _cloudCredentialService;


        public ProjectFeatureService(IDomainManagerService domainManagerService,
                                     IIdentityService identityService,
                                     IUserRepository userRepository,
                                     IEventBusService eventBusService,
                                     IActivityMonitorService activityMonitorService,
                                     IProjectCloudCredentialService cloudCredentialService,
                                     IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _dataProtectorService = dataProtectorService;
            _cloudCredentialService = cloudCredentialService;
        }

        public async Task CreateProjectFeature(Guid organizationId, Guid projectId, ProjectFeaturePostRp resource)
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
                await _domainManagerService.AddForbidden($"You are not authorized to create features in this project.");
                return;
            }

            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to add a new feature.");
                return;
            }

            if(resource.Services.Length == 0)
            {
                await _domainManagerService.AddConflict($"At least one pipe must be included in the feature.");
                return;
            }

            var preparingServices = project.GetPreparingServices();
            if (preparingServices.Any())
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} has pipes in status Preparing. All services must be in status Active to create a new feature");
                return;
            }

            DomainModels.ProjectFeature existingFeature = project.GetFeatureByName(resource.Name);
            if (existingFeature != null)
            {
                await _domainManagerService.AddConflict($"The feature name {resource.Name} has already been taken.");
                return;
            }
            
            DomainModels.ProjectFeature newFeature = user.CreateProjectFeature(organizationId, projectId, resource.Name, resource.Description);

            //services asociated (TODO: services on demand)
            List<ProjectFeatureServiceCreatedEvent> @events = new List<ProjectFeatureServiceCreatedEvent>();
            foreach (var item in resource.Services)
            {
                DomainModels.ProjectService projectService = project.GetServiceById(item);
                if(projectService == null)
                {
                    await _domainManagerService.AddConflict($"The pipe id {item} does not exists.");
                    return;
                }

                var variables = projectService.Environments.First(x => x.ProjectEnvironment.Type == EnvironmentType.Root).Variables;

                newFeature.AddService(item, variables);

                var serviceCredential = this._cloudCredentialService.ProjectServiceCredentialResolver(project, projectService);
                
                @events.Add(new ProjectFeatureServiceCreatedEvent(_correlationId) {
                    ServiceId = item,
                    ServiceExternalId = projectService.ProjectServiceExternalId,
                    ServiceExternalUrl = projectService.ProjectServiceExternalUrl,
                    ServiceName = projectService.Name,
                    InternalServiceName = projectService.InternalName,
                    ServiceTemplateUrl = serviceCredential.BranchUrl,
                    ReleaseStageId = projectService.ReleaseStageId,
                    AgentPoolId = project.AgentPoolId,
                    OrganizationId = organization.OrganizationId,
                    ProjectId = project.ProjectId,
                    FeatureId = newFeature.ProjectFeatureId,
                    ProjectExternalId = serviceCredential.ProjectExternalId,
                    ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                    ProjectExternalGitEndpoint = project.ProjectExternalGitEndpoint,
                    ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                    ProjectVSTSFakeId = project.ProjectVSTSFakeId,
                    OrganizationName = organization.Name,
                    ProjectName = serviceCredential.ProjectName,
                    FeatureName = newFeature.Name,
                    CMSType = serviceCredential.CMSType,
                    CMSAccountId = serviceCredential.AccountId,
                    CMSAccountName = serviceCredential.AccountName,
                    CMSAccessId = serviceCredential.AccessId,
                    CMSAccessSecret = serviceCredential.AccessSecret,
                    CMSAccessToken = serviceCredential.AccessToken,
                    CPSType = project.OrganizationCPS.Type,
                    CPSAccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId),
                    CPSAccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName),
                    CPSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret),
                    CPSAccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion),
                    TemplateAccess = projectService.ProjectServiceTemplate.TemplateAccess,
                    IsImported = projectService.IsImported,
                    NeedCredentials = projectService.ProjectServiceTemplate.NeedCredentials,
                    RepositoryCMSType = serviceCredential.CMSType,
                    RepositoryAccessId = serviceCredential.AccessId,
                    RepositoryAccessSecret = serviceCredential.AccessSecret,
                    RepositoryAccessToken = serviceCredential.AccessToken,
                    UserId = loggedUserId
                });
            }
            
            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            await _domainManagerService.AddResult("FeatureId", newFeature.ProjectFeatureId);

            //send events
            foreach (var @event in @events)
            {
                await _eventBusService.Publish(queueName: "ProjectFeatureServiceCreatedEvent", @event: @event);
            }
        }

        public async Task DeleteProjectFeature(Guid organizationId, Guid projectId, Guid featureId)
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
                await _domainManagerService.AddForbidden($"You are not authorized to delete features in this project.");
                return;
            }

            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to delete the feature.");
                return;
            }

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The feature with id {featureId} does not exists.");
                return;
            }

            if (feature.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The feature with id {featureId} must be in status Active to be deleted.");
                return;
            }

            //services asociated (TODO: services on demand)
            List<ProjectFeatureServiceDeletedEvent> projectFeatureServiceDeletedEventList = new List<ProjectFeatureServiceDeletedEvent>();
            foreach (var item in feature.Services)
            {
                DomainModels.ProjectService projectService = project.GetServiceById(item.ProjectServiceId);
                if (projectService == null)
                {
                    await _domainManagerService.AddConflict($"The pipe id {item} does not exists.");
                    return;
                }

                projectFeatureServiceDeletedEventList.Add(new ProjectFeatureServiceDeletedEvent(_correlationId)
                {
                    ServiceId = item.ProjectServiceId,
                    ServiceExternalId = projectService.ProjectServiceExternalId,
                    ServiceExternalUrl = projectService.ProjectServiceExternalUrl,
                    ServiceName = projectService.Name,
                    ServiceTemplateUrl = projectService.ProjectServiceTemplate.Url,
                    CommitStageId = item.CommitStageId,
                    ReleaseStageId = item.ReleaseStageId,
                    CommitServiceHookId = item.CommitServiceHookId,
                    ReleaseServiceHookId = item.ReleaseServiceHookId,
                    CodeServiceHookId = item.CodeServiceHookId,
                    ReleaseStartedServiceHookId = item.ReleaseStartedServiceHookId,
                    ReleasePendingApprovalServiceHookId = item.ReleasePendingApprovalServiceHookId,
                    ReleaseCompletedApprovalServiceHookId = item.ReleaseCompletedApprovalServiceHookId,
                    OrganizationId = organization.OrganizationId,
                    OrganizationName = organization.Name,
                    ProjectId = project.ProjectId,
                    ProjectExternalId = project.ProjectExternalId,
                    ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                    ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                    ProjectName = project.Name,
                    FeatureId = feature.ProjectFeatureId,
                    FeatureName = feature.Name,
                    CMSType = project.OrganizationCMS.Type,
                    CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                    CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                    CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                    CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                    CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken),
                    CPSType = project.OrganizationCPS.Type,
                    CPSAccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId),
                    CPSAccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName),
                    CPSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret),
                    CPSAccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion),
                    CPSAccessAppId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppId),
                    CPSAccessAppSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppSecret),
                    CPSAccessDirectory = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessDirectory)
                });
            }

            user.DeleteProjectFeature(organizationId, projectId, featureId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            foreach (var item in projectFeatureServiceDeletedEventList)
            {
                await _eventBusService.Publish(queueName: "ProjectFeatureServiceDeletedEvent", @event: item);
            }
        }

        public async Task CompleteProjectFeature(Guid organizationId, Guid projectId, Guid featureId, bool deleteInfrastructure)
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
                await _domainManagerService.AddForbidden($"You are not authorized to complete features in this project.");
                return;
            }

            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to ask for pull request.");
                return;
            }

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The feature with id {featureId} does not exists.");
                return;
            }
            
            if (feature.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The feature with id {featureId} must be in status Active to to ask for pull request.");
                return;
            }

            //if (feature.IsCompleted())
            //{
            //    await _domainManagerService.AddConflict($"The feature with id {featureId} has already been completed.");
            //    return;
            //}

            //services asociated (TODO: services on demand)
            List<ProjectFeatureServiceCompletedEvent> @events = new List<ProjectFeatureServiceCompletedEvent>();
            foreach (var item in feature.Services)
            {
                DomainModels.ProjectService projectService = project.GetServiceById(item.ProjectServiceId);
                if (projectService == null)
                {
                    await _domainManagerService.AddConflict($"The pipe id {item} does not exists.");
                    return;
                }

                @events.Add(new ProjectFeatureServiceCompletedEvent(_correlationId)
                {
                    ServiceId = item.ProjectServiceId,
                    ServiceExternalId = projectService.ProjectServiceExternalId,
                    ServiceExternalUrl = projectService.ProjectServiceExternalUrl,
                    ServiceName = projectService.Name,
                    ServiceTemplateUrl = projectService.ProjectServiceTemplate.Url,
                    CommitStageId = item.CommitStageId,
                    ReleaseStageId = item.ReleaseStageId
                });
            }

            user.CompleteProjectFeature(organizationId, projectId, featureId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            var @event = new ProjectFeatureCompletedEvent(_correlationId)
            {
                OrganizationExternalId = project.OrganizationExternalId,
                OrganizationId = organization.OrganizationId,
                OrganizationName = organization.Name,
                ProjectId = project.ProjectId,
                Services = @events,
                ProjectExternalId = project.ProjectExternalId,
                ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                ProjectName = project.Name,
                FeatureId = feature.ProjectFeatureId,
                FeatureName = feature.Name,
                CMSType = project.OrganizationCMS.Type,
                CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken),
                DeleteInfrastructure = deleteInfrastructure
            };

            //Cloud Provider Data
            @event.CPSType = project.OrganizationCPS.Type;
            @event.CPSAccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId);
            @event.CPSAccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName);
            @event.CPSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret);
            @event.CPSAccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion);
            @event.CPSAccessAppId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppId);
            @event.CPSAccessAppSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppSecret);
            @event.CPSAccessDirectory = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessDirectory);

            await _eventBusService.Publish(queueName: "ProjectFeatureCompletedEvent", @event: @event);
        }
    }
}
