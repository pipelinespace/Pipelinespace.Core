using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using PipelineSpace.Domain.Models;
using PipelineSpace.Worker.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Services
{
    public class ProjectService : IProjectService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IProjectRepository _projectRepository;
        readonly IProjectFeatureRepository _projectFeatureRepository;
        readonly IUserRepository _userRepository;
        readonly IProjectTemplateRepository _projectTemplateRepository;
        readonly Func<ConfigurationManagementService, ICMSService> _cmsService;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IEventBusService _eventBusService;
        readonly ISlugService _slugService;
        readonly string _correlationId;
        readonly IDataProtectorService _dataProtectorService;

        public ProjectService(IDomainManagerService domainManagerService,
                                   IIdentityService identityService,
                                   IOrganizationRepository organizationRepository,
                                   IProjectRepository projectRepository,
                                   IProjectFeatureRepository projectFeatureRepository,
                                   IUserRepository userRepository,
                                   IProjectTemplateRepository projectTemplateRepository,
                                   Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
                                   Func<ConfigurationManagementService, ICMSService> cmsService,
                                   IEventBusService eventBusService,
                                   ISlugService slugService,
                                   IActivityMonitorService activityMonitorService,
                                   IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _projectRepository = projectRepository;
            _projectFeatureRepository = projectFeatureRepository;
            _userRepository = userRepository;
            _projectTemplateRepository = projectTemplateRepository;
            _cmsService = cmsService;
            _cmsCredentialService = cmsCredentialService;
            _eventBusService = eventBusService;
            _slugService = slugService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _dataProtectorService = dataProtectorService;
        }

        public async Task CreateProject(Guid organizationId, ProjectPostRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);
            
            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            OrganizationCMS organizationCMS = organization.GetConfigurationManagementServiceById(resource.OrganizationCMSId);
            if (organizationCMS == null)
            {
                await _domainManagerService.AddNotFound($"The configuration management service with id {resource.OrganizationCMSId} does not exists.");
                return;
            }

            if (organizationCMS.Type == ConfigurationManagementService.VSTS && resource.projectVisibility == ProjectVisibility.None)
            {
                await _domainManagerService.AddConflict($"The project visibility should be Private or Public.");
                return;
            }

            OrganizationCPS organizationCPS = null;
            if (resource.OrganizationCPSId.HasValue)
            {
                organizationCPS = organization.GetCloudProviderServiceById(resource.OrganizationCPSId.Value);

                if (organizationCPS == null)
                {
                    await _domainManagerService.AddNotFound($"The cloud provider service with id {resource.OrganizationCPSId} does not exists.");
                    return;
                }
            }
            else {
                organizationCPS = new OrganizationCPS { Type = CloudProviderService.None };
            }
            
            ProjectTemplate projectTemplate = null;
            if (resource.ProjectTemplateId.HasValue)
            {
                projectTemplate = await _projectTemplateRepository.GetProjectTemplateById(resource.ProjectTemplateId.Value);
                if(projectTemplate == null)
                {
                    await _domainManagerService.AddNotFound($"The project template with id {resource.ProjectTemplateId.Value} does not exists.");
                    return;
                }
                
            }

            Project existingProject = organization.GetProjectByName(resource.Name);
            if (existingProject != null)
            {
                await _domainManagerService.AddConflict($"The project name {resource.Name} has already been taken.");
                return;
            }

            //Auth
            CMSAuthCredentialModel cmsAuthCredential = this._cmsCredentialService(organizationCMS.Type).GetToken(
                                                                _dataProtectorService.Unprotect(organizationCMS.AccountId), 
                                                                _dataProtectorService.Unprotect(organizationCMS.AccountName), 
                                                                _dataProtectorService.Unprotect(organizationCMS.AccessSecret),
                                                                _dataProtectorService.Unprotect(organizationCMS.AccessToken));

            CMSProjectAvailabilityResultModel cmsProjectAvailability = await _cmsService(organizationCMS.Type).ValidateProjectAvailability(cmsAuthCredential, resource.TeamId, resource.Name);

            if (!cmsProjectAvailability.Success)
            {
                await _domainManagerService.AddConflict($"The CMS data is not valid. {cmsProjectAvailability.GetReasonForNoSuccess()}");
                return;
            }

            Project newProject = user.CreateProject(organizationId, resource.TeamId,  resource.Name, resource.Description, resource.ProjectType, resource.OrganizationCMSId, resource.OrganizationCPSId, resource.ProjectTemplateId, resource.AgentPoolId, resource.projectVisibility, organizationCPS.Type, organizationCMS.Type);
            
            //SaveChanges in CSM
            CMSProjectCreateModel projectCreateModel = CMSProjectCreateModel.Factory.Create(organization.Name, resource.Name, resource.Description, resource.projectVisibility);

            projectCreateModel.TeamId = resource.TeamId;

            CMSProjectCreateResultModel cmsProjectCreate = await _cmsService(organizationCMS.Type).CreateProject(cmsAuthCredential, projectCreateModel);

            if (!cmsProjectCreate.Success)
            {
                await _domainManagerService.AddConflict($"The CMS data is not valid. {cmsProjectCreate.GetReasonForNoSuccess()}");
                return;
            }

            newProject.UpdateExternalInformation(cmsProjectCreate.ProjectExternalId, resource.Name);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            await _domainManagerService.AddResult("ProjectId", newProject.ProjectId);

            //send event
            var @event = new ProjectCreatedEvent(_correlationId)
            {
                OrganizationId = organization.OrganizationId,
                ProjectId = newProject.ProjectId,
                ProjectName = newProject.Name,
                InternalProjectName = newProject.InternalName,
                ProjectVSTSFake = this._slugService.GetSlug($"{organization.Owner.Email} {organization.Name} {newProject.Name}"),
                AgentPoolId = newProject.AgentPoolId,

                CMSType = organizationCMS.Type,
                CMSAccountId = _dataProtectorService.Unprotect(organizationCMS.AccountId),
                CMSAccountName = _dataProtectorService.Unprotect(organizationCMS.AccountName),
                CMSAccessId = _dataProtectorService.Unprotect(organizationCMS.AccessId),
                CMSAccessSecret = _dataProtectorService.Unprotect(organizationCMS.AccessSecret),
                CMSAccessToken = _dataProtectorService.Unprotect(organizationCMS.AccessToken),

                CPSType = organizationCPS.Type,
                CPSAccessId = organizationCPS.Type != CloudProviderService.None ? _dataProtectorService.Unprotect(organizationCPS.AccessId) : string.Empty,
                CPSAccessName = organizationCPS.Type != CloudProviderService.None ? _dataProtectorService.Unprotect(organizationCPS.AccessName) : string.Empty,
                CPSAccessSecret = organizationCPS.Type != CloudProviderService.None ? _dataProtectorService.Unprotect(organizationCPS.AccessSecret) : string.Empty,
                CPSAccessAppId = organizationCPS.Type != CloudProviderService.None ? _dataProtectorService.Unprotect(organizationCPS.AccessAppId) : string.Empty,
                CPSAccessAppSecret = organizationCPS.Type != CloudProviderService.None ? _dataProtectorService.Unprotect(organizationCPS.AccessAppSecret) : string.Empty,
                CPSAccessDirectory = organizationCPS.Type != CloudProviderService.None ? _dataProtectorService.Unprotect(organizationCPS.AccessDirectory) : string.Empty,
                UserId = loggedUserId
            };

            if (resource.ProjectTemplateId.HasValue)
            {
                @event.ProjectTemplate = new ProjectTemplateCreatedEvent();
                @event.ProjectTemplate.Services = projectTemplate.Services.Select(x => new ProjectTemplateServiceCreatedEvent() {
                    Name = x.Name,
                    ProjectServiceTemplateId = x.ProjectServiceTemplateId
                }).ToList();
            }

            await _eventBusService.Publish(queueName: "ProjectCreatedEvent", @event: @event);
        }

        public async Task UpdateProjectBasicInformation(Guid organizationId, Guid projectId, ProjectPutRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            Project project = user.FindProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInProject(projectId);
            if (role != PipelineRole.ProjectAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to perform updates in this project.");
                return;
            }

            user.UpdateProject(organizationId, projectId, resource.Name, resource.Description);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
        
        public async Task DeleteProject(Guid organizationId, Guid projectId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            Project project = user.FindProjectById(projectId, false);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInProject(projectId);
            if (role != PipelineRole.ProjectAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to delete this project.");
                return;
            }

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to be modified/deleted.");
                return;
            }

            var preparingServices = project.GetPreparingServices();
            if (preparingServices.Any())
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} has pipes in status Preparing. All services must be in status Active to delete the project");
                return;
            }

            var preparingFeatures = project.GetPreparingFeatures();
            if (preparingFeatures.Any())
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} has features in status Preparing. All features must be in status Active to delete the project");
                return;
            }

            user.DeleteProject(organizationId, projectId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            //send event to delete project in CMS
            var projectDeletedEvent = new ProjectDeletedEvent(_correlationId, project.IsImported)
            {
                OrganizationExternalId = project.OrganizationExternalId,
                ProjectExternalId = project.ProjectExternalId,
                ProjectVSTSFakeId = project.ProjectVSTSFakeId,
                CMSType = project.OrganizationCMS.Type,
                CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken)
            };

            await _eventBusService.Publish(queueName: "ProjectDeletedEvent", @event: projectDeletedEvent);

            //send event to delete cloud services in CPS - Services
            var services = await _projectRepository.GetProjectServices(organizationId, projectId);
            var environments = await _projectRepository.GetProjectEnvironments(organizationId, projectId);
            foreach (var service in services)
            {
                var projectServiceDeletedEvent = new ProjectServiceDeletedEvent(_correlationId)
                {
                    OrganizationExternalId = project.OrganizationExternalId,
                    OrganizationName = organization.Name,
                    ProjectName = project.Name,
                    ServiceName = service.Name,
                    ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                    ProjectExternalId = project.ProjectExternalId,
                    ProjectServiceExternalId = service.ProjectServiceExternalId,
                    CommitStageId = service.CommitStageId,
                    ReleaseStageId = service.ReleaseStageId,
                    CommitServiceHookId = service.CommitServiceHookId,
                    ReleaseServiceHookId = service.ReleaseServiceHookId,
                    CodeServiceHookId = service.CodeServiceHookId,
                    ReleaseStartedServiceHookId = service.ReleaseStartedServiceHookId,
                    ReleasePendingApprovalServiceHookId = service.ReleasePendingApprovalServiceHookId,
                    ReleaseCompletedApprovalServiceHookId = service.ReleaseCompletedApprovalServiceHookId,
                    Environments = environments.Select(x => x.Name).ToList(),
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
                    SourceEvent = Domain.Models.Enums.SourceEvent.Project
                };

                await _eventBusService.Publish(queueName: "ProjectServiceDeletedEvent", @event: projectServiceDeletedEvent);
            }

            //send event to delete cloud services in CPS - Features
            var features = await _projectRepository.GetProjectFeatures(organizationId, projectId);
            foreach (var feature in features)
            {
                List<ProjectFeatureServiceDeletedEvent> projectFeatureServiceDeletedEventList = new List<ProjectFeatureServiceDeletedEvent>();

                var featureServices = await _projectFeatureRepository.GetProjectFeatureServices(organizationId, projectId, feature.ProjectFeatureId);

                foreach (var item in featureServices)
                {
                    projectFeatureServiceDeletedEventList.Add(new ProjectFeatureServiceDeletedEvent(_correlationId)
                    {
                        ServiceId = item.ProjectServiceId,
                        ServiceExternalId = item.ProjectService.ProjectServiceExternalId,
                        ServiceExternalUrl = item.ProjectService.ProjectServiceExternalUrl,
                        ServiceName = item.ProjectService.Name,
                        ServiceTemplateUrl = item.ProjectService.ProjectServiceTemplate.Url,
                        CommitStageId = item.CommitStageId,
                        ReleaseStageId = item.ReleaseStageId,
                        CommitServiceHookId = item.CommitServiceHookId,
                        ReleaseServiceHookId = item.ReleaseServiceHookId,
                        CodeServiceHookId = item.CodeServiceHookId,
                        ReleaseStartedServiceHookId = item.ReleaseStartedServiceHookId,
                        ReleasePendingApprovalServiceHookId = item.ReleasePendingApprovalServiceHookId,
                        ReleaseCompletedApprovalServiceHookId = item.ReleaseCompletedApprovalServiceHookId
                    });
                }

                var projectFeatureDeletedEvent = new ProjectFeatureDeletedEvent(_correlationId)
                {
                    OrganizationId = organization.OrganizationId,
                    OrganizationName = organization.Name,
                    ProjectId = project.ProjectId,
                    Services = projectFeatureServiceDeletedEventList,
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
                    SourceEvent = Domain.Models.Enums.SourceEvent.Project
                };
                
                await _eventBusService.Publish(queueName: "ProjectFeatureDeletedEvent", @event: projectFeatureDeletedEvent);
            }
        }

        public async Task ImportProject(Guid organizationId, ProjectImportPostRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);
            
            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            OrganizationCMS organizationCMS = organization.GetConfigurationManagementServiceById(resource.OrganizationCMSId);
            if (organizationCMS == null)
            {
                await _domainManagerService.AddNotFound($"The configuration management service with id {resource.OrganizationCMSId} does not exists.");
                return;
            }

            if (organizationCMS.Type == ConfigurationManagementService.VSTS && resource.projectVisibility == ProjectVisibility.None)
            {
                await _domainManagerService.AddConflict($"The project visibility should be Private or Public.");
                return;
            }
            
            OrganizationCPS organizationCPS = null;
            if (resource.OrganizationCPSId.HasValue)
            {
                organizationCPS = organization.GetCloudProviderServiceById(resource.OrganizationCPSId.Value);

                if (organizationCPS == null)
                {
                    await _domainManagerService.AddNotFound($"The cloud provider service with id {resource.OrganizationCPSId} does not exists.");
                    return;
                }
            }
            else
            {
                organizationCPS = new OrganizationCPS { Type = CloudProviderService.None };
            }
            
            Project existingProject = organization.GetProjectByName(resource.Name);
            if (existingProject != null)
            {
                await _domainManagerService.AddConflict($"The project name {resource.Name} has already been taken.");
                return;
            }

            //Auth
            CMSAuthCredentialModel cmsAuthCredential = this._cmsCredentialService(organizationCMS.Type).GetToken(
                                                                _dataProtectorService.Unprotect(organizationCMS.AccountId),
                                                                _dataProtectorService.Unprotect(organizationCMS.AccountName),
                                                                _dataProtectorService.Unprotect(organizationCMS.AccessSecret),
                                                                _dataProtectorService.Unprotect(organizationCMS.AccessToken));

            
            Project newProject = user.ImportProject(organizationId, string.Empty, resource.Name, resource.Description, resource.ProjectType, resource.OrganizationCMSId, resource.OrganizationCPSId, null, resource.AgentPoolId, resource.projectVisibility, organizationCPS.Type, organizationCMS.Type);

            //SaveChanges in CSM
            CMSProjectCreateModel projectCreateModel = CMSProjectCreateModel.Factory.Create(organization.Name, resource.Name, resource.Description, resource.projectVisibility);
            
            newProject.UpdateExternalInformation(resource.ProjectExternalId, resource.ProjectExternalName);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            await _domainManagerService.AddResult("ProjectId", newProject.ProjectId);
            
            //send event
            var @event = new ProjectImportedEvent(_correlationId)
            {
                OrganizationId = organization.OrganizationId,
                OrganizationCMSId = resource.OrganizationCMSId,
                ProjectId = newProject.ProjectId,
                ProjectName = resource.Name,
                InternalProjectName = newProject.InternalName,
                ProjectExternalId = resource.ProjectExternalId,
                ProjectExternalName = resource.ProjectExternalName,

                BuildDefinitionYML = resource.BuildDefinitionYML,
                ProjectServiceTemplateId = resource.ProjectServiceTemplateId,

                ProjectVSTSFake = this._slugService.GetSlug($"{organization.Owner.Email} {organization.Name} {newProject.Name}"),
                AgentPoolId = newProject.AgentPoolId,

                CMSType = organizationCMS.Type,
                CMSAccountId = _dataProtectorService.Unprotect(organizationCMS.AccountId),
                CMSAccountName = _dataProtectorService.Unprotect(organizationCMS.AccountName),
                CMSAccessId = _dataProtectorService.Unprotect(organizationCMS.AccessId),
                CMSAccessSecret = _dataProtectorService.Unprotect(organizationCMS.AccessSecret),
                CMSAccessToken = _dataProtectorService.Unprotect(organizationCMS.AccessToken),

                CPSType = organizationCPS.Type,
                CPSAccessId = _dataProtectorService.Unprotect(organizationCPS.AccessId),
                CPSAccessName = _dataProtectorService.Unprotect(organizationCPS.AccessName),
                CPSAccessSecret = _dataProtectorService.Unprotect(organizationCPS.AccessSecret),
                CPSAccessAppId = _dataProtectorService.Unprotect(organizationCPS.AccessAppId),
                CPSAccessAppSecret = _dataProtectorService.Unprotect(organizationCPS.AccessAppSecret),
                CPSAccessDirectory = _dataProtectorService.Unprotect(organizationCPS.AccessDirectory),
                UserId = loggedUserId,

                ProjectRepository = new ProjectRepositoryCreatedEvent {
                    Repositories = resource.Repositories.Select(c=> new ProjectRepositoryServiceCreatedEvent {
                        Id = c.Id,
                        Name = c.Name,
                        Link = c.Link,
                        BranchName = c.BranchName,
                        ExternalName  = c.ExternalName
                    }).ToList(),
                }
            };
            
            await _eventBusService.Publish(queueName: "ProjectImportedEvent", @event: @event);
        }

    }
}
