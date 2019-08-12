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
using PipelineSpace.Application.Interfaces.Models;
using PipelineSpace.Domain.Models;
using PipelineSpace.Worker.Events;
using System.Linq;
using Microsoft.Extensions.Options;
using PipelineSpace.Infra.Options;
using Newtonsoft.Json;
using PipelineSpace.Infra.CrossCutting.Extensions;

namespace PipelineSpace.Application.Services
{
    public class ProjectServiceService : IProjectServiceService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IUserRepository _userRepository;
        readonly Func<ConfigurationManagementService, ICMSService> _cmsService;
        readonly Func<ConfigurationManagementService, ICMSCredentialService> _cmsCredentialService;
        readonly IProjectServiceTemplateRepository _projectServiceTemplateRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly ICMSPipelineService _cmsPipelineService;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IDataProtectorService _dataProtectorService;
        readonly IProjectCloudCredentialService _cloudCredentialService;

        public ProjectServiceService(IDomainManagerService domainManagerService,
                                     IIdentityService identityService,
                                     IOrganizationRepository organizationRepository,
                                     IUserRepository userRepository,
                                     Func<ConfigurationManagementService, ICMSService> cmsService,
                                     Func<ConfigurationManagementService, ICMSCredentialService> cmsCredentialService,
                                     IProjectServiceTemplateRepository projectServiceTemplateRepository,
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
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _projectServiceTemplateRepository = projectServiceTemplateRepository;
            _cmsService = cmsService;
            _cmsCredentialService = cmsCredentialService;
            _cmsPipelineService = cmsPipelineService;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _vstsOptions = vstsOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _dataProtectorService = dataProtectorService;
            _cloudCredentialService = cloudCredentialService;
        }

        public async Task CreateProjectService(Guid organizationId, Guid projectId, ProjectServicePostRp resource, string userId = null)
        {
            string loggedUserId = userId ?? _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);
            
            DomainModels.ProjectServiceTemplate  projectServiceTemplate = await  _projectServiceTemplateRepository.GetProjectServiceTemplateById(resource.ProjectServiceTemplateId);
            if (projectServiceTemplate == null)
            {
                await _domainManagerService.AddConflict($"The pipe template with id {resource.ProjectServiceTemplateId} does not exists.");
                return;
            }

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = user.FindProjectById(projectId, false);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to add a new service.");
                return;
            }

            DomainModels.ProjectService existingService = project.GetServiceByName(resource.Name);
            if (existingService != null)
            {
                await _domainManagerService.AddConflict($"The pipe name {resource.Name} has already been taken.");
                return;
            }

            CMSAuthCredentialModel cmsAuthCredential = this._cmsCredentialService(project.OrganizationCMS.Type).GetToken(
                                                                _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId), 
                                                                _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName), 
                                                                _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                                                                _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken));

            CMSServiceAvailabilityResultModel cmsServiceAvailability = await _cmsService(project.OrganizationCMS.Type).ValidateServiceAvailability(cmsAuthCredential, project.OrganizationExternalId, project.ProjectExternalId, project.Name, resource.RepositoryName);

            if (!cmsServiceAvailability.Success)
            {
                await _domainManagerService.AddConflict($"The CMS data is not valid. {cmsServiceAvailability.GetReasonForNoSuccess()}");
                return;
            }

            DomainModels.ProjectService newService = user.CreateProjectService(organizationId, projectId, project.OrganizationCMSId, resource.AgentPoolId, resource.Name, resource.RepositoryName, resource.Description, resource.ProjectServiceTemplateId, projectServiceTemplate.PipeType);

            //SaveChanges in CMS
            CMSServiceCreateModel serviceCreateModel = CMSServiceCreateModel.Factory.Create(project.OrganizationExternalId, project.ProjectExternalId, project.Name, resource.RepositoryName, project.ProjectVisibility == ProjectVisibility.Public ? true : false);
            CMSServiceCreateResultModel cmsServiceCreate = await _cmsService(project.OrganizationCMS.Type).CreateService(cmsAuthCredential, serviceCreateModel);

            if (!cmsServiceCreate.Success)
            {
                await _domainManagerService.AddConflict($"The CMS data is not valid. {cmsServiceCreate.GetReasonForNoSuccess()}");
                return;
            } 

            newService.UpdateExternalInformation(cmsServiceCreate.ServiceExternalId, cmsServiceCreate.ServiceExternalUrl, resource.Name);
            newService.AddEnvironmentsAndVariables(projectServiceTemplate.Parameters);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            await _domainManagerService.AddResult("ServiceId", newService.ProjectServiceId);

            if (project.OrganizationCPS == null)
                project.OrganizationCPS = new OrganizationCPS { Type = CloudProviderService.None };

            var @event = new ProjectServiceCreatedEvent(_correlationId)
            {
                OrganizationId = organization.OrganizationId,
                OrganizationName = organization.Name,
                ProjectId = project.ProjectId,
                ServiceId = newService.ProjectServiceId,
                ProjectExternalId = project.ProjectExternalId,
                ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                ProjectExternalGitEndpoint = project.ProjectExternalGitEndpoint,
                ProjectVSTSFakeId = project.ProjectVSTSFakeId,
                ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                ProjectName = project.Name,
                InternalProjectName = project.InternalName,
                AgentPoolId = newService.AgentPoolId,
                ServiceExternalId = newService.ProjectServiceExternalId,
                ServiceExternalUrl = newService.ProjectServiceExternalUrl,
                ServiceName = resource.Name,
                InternalServiceName = newService.InternalName,
                ServiceTemplateUrl = projectServiceTemplate.Url,
                CMSType = project.OrganizationCMS.Type,
                CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken),
                UserId = loggedUserId,
                TemplateParameters = projectServiceTemplate.Parameters.Select(x => new ProjectServiceTemplateParameterCreatedEvent() {
                    VariableName = x.VariableName,
                    Value = x.Value,
                    Scope = x.Scope
                }).ToList(),
                CPSType = project.OrganizationCPS.Type,
                CPSAccessId = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId),
                CPSAccessName = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName),
                CPSAccessSecret = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret),
                CPSAccessRegion = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion),
                TemplateAccess = projectServiceTemplate.TemplateAccess,
                NeedCredentials = projectServiceTemplate.NeedCredentials,
                RepositoryCMSType = projectServiceTemplate.TemplateAccess == DomainModels.Enums.TemplateAccess.Organization ? projectServiceTemplate.Credential.CMSType : ConfigurationManagementService.VSTS,
                RepositoryAccessId = projectServiceTemplate.TemplateAccess == DomainModels.Enums.TemplateAccess.Organization ? projectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectServiceTemplate.Credential.AccessId) : string.Empty : string.Empty,
                RepositoryAccessSecret = projectServiceTemplate.TemplateAccess == DomainModels.Enums.TemplateAccess.Organization ? projectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectServiceTemplate.Credential.AccessSecret) : string.Empty : string.Empty,
                RepositoryAccessToken = projectServiceTemplate.TemplateAccess == DomainModels.Enums.TemplateAccess.Organization ? projectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectServiceTemplate.Credential.AccessToken) : string.Empty : string.Empty
            };

            //Cloud Provider Data


            await _eventBusService.Publish(queueName: "ProjectServiceCreatedEvent", @event: @event);
        }

        public async Task UpdateProjectService(Guid organizationId, Guid projectId, Guid serviceId, ProjectServicePutRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to modify a service.");
                return;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The project pipe with id {serviceId} does not exists.");
                return;
            }

            user.UpdateProjectService(organizationId, projectId, serviceId, resource.Name, resource.Description);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }

        public async Task DeleteProjectService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to modify a service.");
                return;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The project pipe with id {serviceId} does not exists.");
                return;
            }

            if (service.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The pipe with id {serviceId} must be in status Active to be modified/deleted.");
                return;
            }

            /*Check If any feature is associated with the service*/
            var features = project.GetFeatures();
            foreach (var feature in features)
            {
                var featureService = feature.GetFeatureServiceById(serviceId);
                if(featureService != null)
                {
                    await _domainManagerService.AddConflict($"The are active features ({feature.Name}) using the service, you cannot delete the service.");
                    return;
                }

            }

            user.DeleteProjectService(organizationId, projectId, serviceId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            if (project.OrganizationCPS == null)
                project.OrganizationCPS = new OrganizationCPS { Type = CloudProviderService.None };

            //send event
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
                CodeServiceHookId = service.CodeServiceHookId,
                ReleaseStartedServiceHookId = service.ReleaseStartedServiceHookId,
                ReleasePendingApprovalServiceHookId = service.ReleasePendingApprovalServiceHookId,
                ReleaseCompletedApprovalServiceHookId = service.ReleaseCompletedApprovalServiceHookId,
                ReleaseServiceHookId = service.ReleaseServiceHookId,
                Environments = project.GetEnvironments().Select(x=> x.Name).ToList(),
                CMSType = project.OrganizationCMS.Type,
                CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken),
                CPSType = project.OrganizationCPS.Type,
                CPSAccessId =  project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId),
                CPSAccessName = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName),
                CPSAccessSecret = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret),
                CPSAccessRegion = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion),
                CPSAccessAppId = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppId),
                CPSAccessAppSecret = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessAppSecret),
                CPSAccessDirectory = project.OrganizationCPS.Type == CloudProviderService.None ? string.Empty : _dataProtectorService.Unprotect(project.OrganizationCPS.AccessDirectory),
                SourceEvent = DomainModels.Enums.SourceEvent.Service
            };

            await _eventBusService.Publish(queueName: "ProjectServiceDeletedEvent", @event: projectServiceDeletedEvent);
        }

        //actions
        public async Task CreateBuildProjectService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to modify a service.");
                return;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The project pipe with id {serviceId} does not exists.");
                return;
            }
            
            if (service.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The pipe with id {serviceId} must be in status Active to request a Build.");
                return;
            }

            var serviceCredential = this._cloudCredentialService.ProjectServiceCredentialResolver(project, service);

            CMSPipelineAgentQueueParamModel getQueueOptions = new CMSPipelineAgentQueueParamModel();
            getQueueOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            getQueueOptions.CMSType = serviceCredential.CMSType;
            getQueueOptions.VSTSAccountName = serviceCredential.AccountName;
            getQueueOptions.VSTSAccessSecret = serviceCredential.AccessSecret;
            getQueueOptions.VSTSAccountProjectId = serviceCredential.AccountProjectId;

            getQueueOptions.ProjectName = serviceCredential.ProjectName;
            getQueueOptions.AgentPoolId = project.AgentPoolId;

            var queue = await _cmsPipelineService.GetQueue(getQueueOptions);

            if(queue == null)
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
            queueBuildOptions.BuildDefinitionId = service.CommitStageId.Value;
            queueBuildOptions.SourceBranch = service.BranchName;

            await _cmsPipelineService.CreateBuild(queueBuildOptions);

            var @event = new ProjectServiceBuildQueuedEvent(_correlationId)
            {
                OrganizationId = organization.OrganizationId,
                ProjectId = project.ProjectId,
                ServiceId = service.ProjectServiceId
            };

            await _eventBusService.Publish(queueName: "ProjectServiceBuildQueuedEvent", @event: @event);
        }

        public async Task CreateReleaseProjectService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to modify a service.");
                return;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The project pipe with id {serviceId} does not exists.");
                return;
            }

            if (service.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The pipe with id {serviceId} must be in status Active to request a Request.");
                return;
            }

            if (string.IsNullOrEmpty(service.LastBuildSuccessVersionId))
            {
                await _domainManagerService.AddConflict($"The project service with id {serviceId} does not have any success build yet.");
                return;
            }

            var serviceCredential = this._cloudCredentialService.ProjectServiceCredentialResolver(project, service);

            CMSPipelineReleaseParamModel releaseBuildOptions = new CMSPipelineReleaseParamModel();
            releaseBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            releaseBuildOptions.VSTSAccountName = serviceCredential.AccountName;
            releaseBuildOptions.VSTSAccessSecret = serviceCredential.AccessSecret;
            releaseBuildOptions.VSTSAccountProjectId = serviceCredential.AccountProjectId;

            releaseBuildOptions.ProjectName = serviceCredential.ProjectName;
            releaseBuildOptions.ProjectExternalId = serviceCredential.ProjectExternalId;
            releaseBuildOptions.ReleaseDefinitionId = service.ReleaseStageId.Value;
            releaseBuildOptions.Alias = service.Name;
            
            releaseBuildOptions.VersionId = int.Parse(service.LastBuildSuccessVersionId);
            releaseBuildOptions.VersionName = service.LastBuildSuccessVersionName;
            releaseBuildOptions.Description = "Release created from PipelineSpace";

            await _cmsPipelineService.CreateRelease(releaseBuildOptions);
        }

        public async Task CompleteApprovalProjectService(Guid organizationId, Guid projectId, Guid serviceId, int approvalId, ProjectServiceApprovalPutRp resource)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            DomainModels.Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            DomainModels.Project project = organization.GetProjectById(projectId);
            if (project == null)
            {
                await _domainManagerService.AddNotFound($"The project with id {projectId} does not exists.");
                return;
            }

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to modify a service.");
                return;
            }

            DomainModels.ProjectService service = project.GetServiceById(serviceId);
            if (service == null)
            {
                await _domainManagerService.AddNotFound($"The project pipe with id {serviceId} does not exists.");
                return;
            }

            if (service.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The pipe with id {serviceId} must be in status Active to complete an approval.");
                return;

            }
            CMSPipelineApprovalParamModel completeApprovalOptions = new CMSPipelineApprovalParamModel();
            completeApprovalOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            completeApprovalOptions.VSTSAccountName = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName) : _fakeAccountOptions.Value.AccountId;
            completeApprovalOptions.VSTSAccessSecret = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret) : _fakeAccountOptions.Value.AccessSecret;
            completeApprovalOptions.VSTSAccountProjectId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.Name : project.ProjectVSTSFakeName;

            completeApprovalOptions.ProjectName = project.Name;
            completeApprovalOptions.ProjectExternalId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.ProjectExternalId : project.ProjectVSTSFakeId;
            completeApprovalOptions.ApprovalId = approvalId;
            completeApprovalOptions.Status = resource.Status;
            completeApprovalOptions.Comments = resource.Comments;

            await _cmsPipelineService.CompleteApproval(completeApprovalOptions);
        }

        public async Task ImportProjectService(Guid organizationId, Guid projectId, ProjectServiceImportPostRp resource, string userId = null)
        {
            string loggedUserId = userId ?? _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);
            
            DomainModels.ProjectServiceTemplate projectServiceTemplate = await _projectServiceTemplateRepository.GetProjectServiceTemplateById(resource.ProjectServiceTemplateId);
            if (projectServiceTemplate == null)
            {
                await _domainManagerService.AddConflict($"The pipe template with id {resource.ProjectServiceTemplateId} does not exists.");
                return;
            }

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

            if (project.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to add a new service.");
                return;
            }

            DomainModels.ProjectService existingService = project.GetServiceByName(resource.Name);
            if (existingService != null)
            {
                await _domainManagerService.AddConflict($"The pipe name {resource.Name} has already been taken.");
                return;
            }

            var gitProviders = organization.GetConfigurationManagementServices(DomainModels.Enums.CMSConnectionType.ProjectLevel);
            var cms = gitProviders.FirstOrDefault(c => c.OrganizationCMSId.Equals(resource.OrganizationCMSId));

            CMSAuthCredentialModel cmsAuthCredential = this._cmsCredentialService(cms.Type).GetToken(
                                                                _dataProtectorService.Unprotect(cms.AccountId),
                                                                _dataProtectorService.Unprotect(cms.AccountName),
                                                                _dataProtectorService.Unprotect(cms.AccessSecret),
                                                                _dataProtectorService.Unprotect(cms.AccessToken));
            
            DomainModels.ProjectService newService = user.ImportProjectService(organizationId,
                projectId, cms.OrganizationCMSId, resource.AgentPoolId, resource.Name, resource.Name, resource.Description, 
                resource.ProjectServiceTemplateId,  projectServiceTemplate.PipeType, resource.BranchName, resource.ServiceExternalUrl, 
                resource.ProjectExternalId, resource.ProjectExternalName);

            //SaveChanges in CMS
            CMSServiceCreateModel serviceCreateModel = CMSServiceCreateModel.Factory.Create(project.OrganizationExternalId, project.ProjectExternalId, project.Name, resource.Name, project.ProjectVisibility == ProjectVisibility.Public ? true : false);
            
            newService.UpdateExternalInformation(resource.ServiceExternalId, resource.ServiceExternalUrl, resource.ServiceExternalName);
            newService.AddEnvironmentsAndVariables(projectServiceTemplate.Parameters);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            await _domainManagerService.AddResult("ServiceId", newService.ProjectServiceId);

            var @event = new ProjectServiceImportedEvent(_correlationId)
            {
                BranchName = resource.BranchName,
                OrganizationId = organization.OrganizationId,
                OrganizationName = organization.Name,
                ProjectId = project.ProjectId,
                ServiceId = newService.ProjectServiceId,
                ProjectExternalId = resource.ProjectExternalId,
                ProjectExternalName = resource.ProjectExternalName,
                ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                ProjectExternalGitEndpoint = project.ProjectExternalGitEndpoint,
                ProjectVSTSFakeId = project.ProjectVSTSFakeId,
                ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                ProjectName = project.Name,
                AgentPoolId = newService.AgentPoolId,
                ServiceExternalId = newService.ProjectServiceExternalId,
                ServiceExternalUrl = newService.ProjectServiceExternalUrl,
                ServiceName = resource.Name,
                InternalServiceName = newService.InternalName,
                ServiceTemplateUrl = projectServiceTemplate.Url,
                ServiceTemplatePath = projectServiceTemplate.Path,
                BuildDefinitionYML = resource.BuildDefinitionYML,
                
                CMSType = cms.Type,
                CMSAccountId = _dataProtectorService.Unprotect(cms.AccountId),
                CMSAccountName = _dataProtectorService.Unprotect(cms.AccountName),
                CMSAccessId = _dataProtectorService.Unprotect(cms.AccessId),
                CMSAccessSecret = _dataProtectorService.Unprotect(cms.AccessSecret),
                CMSAccessToken = _dataProtectorService.Unprotect(cms.AccessToken),
                UserId = loggedUserId,
                TemplateParameters = projectServiceTemplate.Parameters.Select(x => new ProjectServiceTemplateParameterCreatedEvent()
                {
                    VariableName = x.VariableName,
                    Value = x.Value,
                    Scope = x.Scope
                }).ToList(),
                CPSType = project.OrganizationCPS.Type,
                CPSAccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId),
                CPSAccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName),
                CPSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret),
                CPSAccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion),
                TemplateAccess = projectServiceTemplate.TemplateAccess,
                NeedCredentials = projectServiceTemplate.NeedCredentials,
                RepositoryCMSType = projectServiceTemplate.TemplateAccess == DomainModels.Enums.TemplateAccess.Organization ? projectServiceTemplate.Credential.CMSType : ConfigurationManagementService.VSTS,
                RepositoryAccessId = projectServiceTemplate.TemplateAccess == DomainModels.Enums.TemplateAccess.Organization ? projectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectServiceTemplate.Credential.AccessId) : string.Empty : string.Empty,
                RepositoryAccessSecret = projectServiceTemplate.TemplateAccess == DomainModels.Enums.TemplateAccess.Organization ? projectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectServiceTemplate.Credential.AccessSecret) : string.Empty : string.Empty,
                RepositoryAccessToken = projectServiceTemplate.TemplateAccess == DomainModels.Enums.TemplateAccess.Organization ? projectServiceTemplate.NeedCredentials ? _dataProtectorService.Unprotect(projectServiceTemplate.Credential.AccessToken) : string.Empty : string.Empty
            };

            //Cloud Provider Data
            
            await _eventBusService.Publish(queueName: "ProjectServiceImportedEvent", @event: @event);
        }
    }
}
