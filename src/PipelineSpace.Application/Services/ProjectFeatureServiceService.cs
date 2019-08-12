using Microsoft.Extensions.Options;
using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModels = PipelineSpace.Domain.Models;

namespace PipelineSpace.Application.Services
{
    public class ProjectFeatureServiceService : IProjectFeatureServiceService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly ICMSPipelineService _cmsPipelineService;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IDataProtectorService _dataProtectorService;
        readonly IProjectCloudCredentialService _cloudCredentialService;

        public ProjectFeatureServiceService(IDomainManagerService domainManagerService,
                                            IIdentityService identityService,
                                            IUserRepository userRepository,
                                            ICMSPipelineService cmsPipelineService,
                                            IEventBusService eventBusService,
                                            IActivityMonitorService activityMonitorService,
                                            IOptions<VSTSServiceOptions> vstsOptions,
                                            IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                                IProjectCloudCredentialService cloudCredentialService,
                                            IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
            _cmsPipelineService = cmsPipelineService;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _vstsOptions = vstsOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _dataProtectorService = dataProtectorService;
            _cloudCredentialService = cloudCredentialService;
        }

        public async Task CreateProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, ProjectFeatureServicePostRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

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

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to add a new feature service.");
                return;
            }

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            if (feature.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project feature with id {featureId} must be in status Active to add a new feature service.");
                return;
            }

            if (resource.Services.Length == 0)
            {
                await _domainManagerService.AddConflict($"At least one pipe must be included.");
                return;
            }

            List<ProjectFeatureServiceCreatedEvent> projectFeatureServiceCreatedEventList = new List<ProjectFeatureServiceCreatedEvent>();
            foreach (var item in resource.Services)
            {
                DomainModels.ProjectService projectService = project.GetServiceById(item);
                if (projectService == null)
                {
                    await _domainManagerService.AddConflict($"The pipe with id {item} does not exists.");
                    return;
                }

                if(projectService.Status != EntityStatus.Active)
                {
                    await _domainManagerService.AddConflict($"The pipe with id {item} must be in status Active to be added as a feature service.");
                    return;
                }

                DomainModels.ProjectFeatureService projectFeatureService = feature.GetFeatureServiceById(item);
                if (projectFeatureService != null)
                {
                    await _domainManagerService.AddConflict($"The pipe with id {item} already exists in the feature.");
                    return;
                }

                var variables = projectService.Environments.First(x => x.ProjectEnvironment.Type == EnvironmentType.Root).Variables;
                
                feature.AddService(item, variables);

                projectFeatureServiceCreatedEventList.Add(new ProjectFeatureServiceCreatedEvent(_correlationId)
                {
                    OrganizationId = organization.OrganizationId,
                    ProjectId = project.ProjectId,
                    FeatureId = feature.ProjectFeatureId,
                    ProjectExternalId = project.ProjectExternalId,
                    ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                    ProjectExternalGitEndpoint = project.ProjectExternalGitEndpoint,
                    ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                    ProjectVSTSFakeId = project.ProjectVSTSFakeId,
                    OrganizationName = organization.Name,
                    ProjectName = project.Name,
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
                    ServiceId = item,
                    ServiceExternalId = projectService.ProjectServiceExternalId,
                    ServiceExternalUrl = projectService.ProjectServiceExternalUrl,
                    ServiceName = projectService.Name,
                    InternalServiceName = projectService.InternalName,
                    ServiceTemplateUrl = projectService.ProjectServiceTemplate.Url,
                    ReleaseStageId = projectService.ReleaseStageId,
                    AgentPoolId = project.AgentPoolId,
                    UserId = loggedUserId
                });
            }

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            //send events
            foreach (var @event in projectFeatureServiceCreatedEventList)
            {
                await _eventBusService.Publish(queueName: "ProjectFeatureServiceCreatedEvent", @event: @event);
            }
        }

        public async Task DeleteProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

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

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to delete a feature service.");
                return;
            }

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            if (feature.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project feature with id {featureId} must be in status Active to delete a feature service.");
                return;
            }

            DomainModels.ProjectFeatureService featureService = feature.GetFeatureServiceById(serviceId);
            if (featureService == null)
            {
                await _domainManagerService.AddNotFound($"The feature pipe with id {serviceId} does not exists.");
                return;
            }

            var services = feature.GetServices();
            if(services.Count == 1)
            {
                await _domainManagerService.AddConflict($"The project feature with id {featureId} must have at least one pipe, anycase you could delete the feature instead.");
                return;
            }

            feature.DeleteService(serviceId, loggedUserId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            var @event = new ProjectFeatureServiceDeletedEvent(_correlationId)
            {
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
                CPSAccessDirectory = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessDirectory),
                ServiceId = featureService.ProjectServiceId,
                ServiceExternalId = featureService.ProjectService.ProjectServiceExternalId,
                ServiceExternalUrl = featureService.ProjectService.ProjectServiceExternalUrl,
                ServiceName = featureService.ProjectService.Name,
                ServiceTemplateUrl = featureService.ProjectService.ProjectServiceTemplate.Url,
                CommitStageId = featureService.CommitStageId,
                ReleaseStageId = featureService.ReleaseStageId,
                CommitServiceHookId = featureService.CommitServiceHookId,
                ReleaseServiceHookId = featureService.ReleaseServiceHookId,
                CodeServiceHookId = featureService.CodeServiceHookId,
                ReleaseStartedServiceHookId = featureService.ReleaseStartedServiceHookId,
                ReleasePendingApprovalServiceHookId = featureService.ReleasePendingApprovalServiceHookId,
                ReleaseCompletedApprovalServiceHookId = featureService.ReleaseCompletedApprovalServiceHookId
            };

            await _eventBusService.Publish(queueName: "ProjectFeatureServiceDeletedEvent", @event: @event);
        }

        public async Task CreateBuildProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

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

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to delete a feature service.");
                return;
            }

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            if (feature.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project feature with id {featureId} must be in status Active to delete a feature service.");
                return;
            }

            DomainModels.ProjectFeatureService featureService = feature.GetFeatureServiceById(serviceId);
            if (featureService == null)
            {
                await _domainManagerService.AddNotFound($"The feature pipe with id {serviceId} does not exists.");
                return;
            }

            var serviceCredential = this._cloudCredentialService.ProjectFeatureServiceCredentialResolver(project, featureService);
            
            CMSPipelineAgentQueueParamModel getQueueOptions = new CMSPipelineAgentQueueParamModel();
            getQueueOptions.CMSType = serviceCredential.CMSType;
            getQueueOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            getQueueOptions.VSTSAccountName = serviceCredential.AccountName;
            getQueueOptions.VSTSAccessSecret = serviceCredential.AccessSecret;
            getQueueOptions.VSTSAccountProjectId = serviceCredential.AccountProjectId ;

            getQueueOptions.ProjectName = serviceCredential.ProjectName;
            getQueueOptions.AgentPoolId = project.AgentPoolId;

            var queue = await _cmsPipelineService.GetQueue(getQueueOptions);

            if (queue == null)
            {
                await _domainManagerService.AddConflict($"The agent pool id {project.AgentPoolId} is not available.");
                return;
            }

            
            CMSPipelineBuildParamModel queueBuildOptions = new CMSPipelineBuildParamModel();
            queueBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            queueBuildOptions.VSTSAccountName = serviceCredential.AccountName;
            queueBuildOptions.VSTSAccessSecret = serviceCredential.AccessSecret;
            queueBuildOptions.VSTSAccountProjectId = serviceCredential.AccountProjectId;

            queueBuildOptions.ProjectName = serviceCredential.ProjectName;
            queueBuildOptions.ProjectExternalId = serviceCredential.ProjectExternalId;
            queueBuildOptions.QueueId = queue.QueueId;
            queueBuildOptions.BuildDefinitionId = featureService.CommitStageId.Value;
            queueBuildOptions.SourceBranch = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? $"refs/heads/{feature.Name.ToLower()}" : feature.Name.ToLower();

            await _cmsPipelineService.CreateBuild(queueBuildOptions);

            var @event = new ProjectFeatureServiceBuildQueuedEvent(_correlationId)
            {
                OrganizationId = organization.OrganizationId,
                ProjectId = project.ProjectId,
                FeatureId = feature.ProjectFeatureId,
                ServiceId = featureService.ProjectServiceId
            };

            await _eventBusService.Publish(queueName: "ProjectFeatureServiceBuildQueuedEvent", @event: @event);

        }

        public async Task CreateReleaseProjectFeatureService(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

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

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to delete a feature service.");
                return;
            }

            DomainModels.ProjectFeature feature = project.GetFeatureById(featureId);
            if (feature == null)
            {
                await _domainManagerService.AddNotFound($"The project feature with id {featureId} does not exists.");
                return;
            }

            if (feature.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project feature with id {featureId} must be in status Active to delete a feature service.");
                return;
            }

            DomainModels.ProjectFeatureService featureService = feature.GetFeatureServiceById(serviceId);
            if (featureService == null)
            {
                await _domainManagerService.AddNotFound($"The feature pipe with id {serviceId} does not exists.");
                return;
            }

            if (string.IsNullOrEmpty(featureService.LastBuildSuccessVersionId))
            {
                await _domainManagerService.AddConflict($"The feature service with id {serviceId} does not have any success build yet.");
                return;
            }

            CMSPipelineReleaseParamModel releaseBuildOptions = new CMSPipelineReleaseParamModel();
            releaseBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            releaseBuildOptions.VSTSAccountName = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName) : _fakeAccountOptions.Value.AccountId;
            releaseBuildOptions.VSTSAccessSecret = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret) : _fakeAccountOptions.Value.AccessSecret;
            releaseBuildOptions.VSTSAccountProjectId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.Name : project.ProjectVSTSFakeName;

            releaseBuildOptions.ProjectName = project.Name;
            releaseBuildOptions.ProjectExternalId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.ProjectExternalId : project.ProjectVSTSFakeId;
            releaseBuildOptions.ReleaseDefinitionId = featureService.ReleaseStageId.Value;
            releaseBuildOptions.Alias = $"{featureService.ProjectService.Name}-ft-{feature.Name.ToLower()}";
            
            releaseBuildOptions.VersionId = int.Parse(featureService.LastBuildSuccessVersionId);
            releaseBuildOptions.VersionName = featureService.LastBuildSuccessVersionName;
            releaseBuildOptions.Description = "Release created from PipelineSpace";

            await _cmsPipelineService.CreateRelease(releaseBuildOptions);

        }
    }
}
