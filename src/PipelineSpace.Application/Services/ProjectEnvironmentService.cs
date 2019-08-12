using PipelineSpace.Application.Interfaces;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Interfaces;
using System;
using System.Collections.Generic;
using DomainModels = PipelineSpace.Domain.Models;
using System.Threading.Tasks;
using System.Linq;
using PipelineSpace.Worker.Events;
using PipelineSpace.Application.Interfaces.Models;
using Microsoft.Extensions.Options;
using PipelineSpace.Infra.Options;
using PipelineSpace.Domain.Models;

namespace PipelineSpace.Application.Services
{
    public class ProjectEnvironmentService : IProjectEnvironmentService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly ICMSPipelineService _cmsPipelineService;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IDataProtectorService _dataProtectorService;

        public ProjectEnvironmentService(IDomainManagerService domainManagerService,
                                         IIdentityService identityService,
                                         IUserRepository userRepository,
                                         IEventBusService eventBusService,
                                         IActivityMonitorService activityMonitorService,
                                         ICMSPipelineService cmsPipelineService,
                                         IOptions<VSTSServiceOptions> vstsOptions,
                                         IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                         IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _cmsPipelineService = cmsPipelineService;
            _vstsOptions = vstsOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _dataProtectorService = dataProtectorService;
        }

        public async Task CreateProjectEnvironment(Guid organizationId, Guid projectId, ProjectEnvironmentPostRp resource)
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
                await _domainManagerService.AddForbidden($"You are not authorized to create environments in this project.");
                return;
            }


            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to add a new feature.");
                return;
            }

            var activeServices = project.GetServicesWithReleaseStages();
            if (!activeServices.Any())
            {
                await _domainManagerService.AddConflict($"At least one pipe must be configured in the project.");
                return;
            }

            DomainModels.ProjectEnvironment existingEnvironment = project.GetEnvironmentByName(resource.Name);
            if (existingEnvironment != null)
            {
                await _domainManagerService.AddConflict($"The environment name {resource.Name} has already been taken.");
                return;
            }

            DomainModels.ProjectEnvironment newEnvironment = user.CreateProjectEnvironment(organizationId, projectId, resource.Name, resource.Description, resource.RequiresApproval, resource.AutoProvision);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            await _domainManagerService.AddResult("EnvironmentId", newEnvironment.ProjectEnvironmentId);
        }

        public async Task CreateProjectEnvironmentVariables(Guid organizationId, Guid projectId, Guid environmentId, ProjectEnvironmentVariablePostRp resource)
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

            DomainModels.ProjectEnvironment environment = project.GetEnvironmentById(environmentId);
            if (environment == null)
            {
                await _domainManagerService.AddNotFound($"The environment with id {environmentId} does not exists.");
                return;
            }

            bool autoProvision = false;
            if(environment.Type == DomainModels.EnvironmentType.Root)
            {
                foreach (var resourceVariable in resource.Items)
                {
                    if (string.IsNullOrEmpty(resourceVariable.Name) || string.IsNullOrEmpty(resourceVariable.Value))
                    {
                        await _domainManagerService.AddConflict($"The environment variable name/value is mandatory.");
                        return;
                    }

                    var variable = environment.GetVariableByName(resourceVariable.Name);
                    if(variable != null)
                    {
                        environment.SetVariable(resourceVariable.Name, resourceVariable.Value);
                    }
                    else
                    {
                        environment.AddVariable(resourceVariable.Name, resourceVariable.Value);
                    }
                }
            }
            else
            {
                DomainModels.ProjectEnvironment rootEnvironment = project.GetRootEnvironment();

                foreach (var variable in rootEnvironment.Variables)
                {
                    var resourceVariable = resource.Items.FirstOrDefault(x => x.Name.Equals(variable.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (resourceVariable == null)
                    {
                        await _domainManagerService.AddConflict($"The environment variable {variable.Name} is mandatory.");
                        return;
                    }

                    if (string.IsNullOrEmpty(resourceVariable.Value))
                    {
                        await _domainManagerService.AddConflict($"The environment variable value {variable.Name} is mandatory.");
                        return;
                    }

                    var existingVariable = environment.GetVariableByName(resourceVariable.Name);
                    if (existingVariable != null)
                    {
                        environment.SetVariable(resourceVariable.Name, resourceVariable.Value);
                    }
                    else
                    {
                        environment.AddVariable(resourceVariable.Name, resourceVariable.Value);
                    }
                }

                if(environment.Status == DomainModels.EntityStatus.Preparing)
                {
                    autoProvision = environment.AutoProvision;
                }

                environment.Activate();
            }

            var projectServices = project.GetServicesWithReleaseStages();

            //replicate service environments
            foreach (var projectService in projectServices)
            {
                var rootVariables = projectService.GetRootEnvironmentVariables();
                projectService.AddEnvironment(environment.ProjectEnvironmentId, rootVariables);
            }

            _userRepository.Update(user);
            await _userRepository.SaveChanges();

            //send events
            foreach (var projectService in projectServices)
            {
                var @event = new ProjectEnvironmentCreatedEvent(_correlationId)
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
                    EnvironmentId = environment.ProjectEnvironmentId,
                    EnvironmentName = environment.Name,
                    EnvironmentRank = environment.Rank,
                    EnvironmentAutoProvision = autoProvision,
                    ReleseStageId = projectService.ReleaseStageId.Value,
                    ServiceName = projectService.Name,
                    ServiceLastBuildSuccessVersionId = projectService.LastBuildSuccessVersionId,
                    ServiceLastBuildSuccessVersionName = projectService.LastBuildSuccessVersionName
                };

                @event.Environments = new List<ProjectEnvironmentItemCreatedEvent>();

                foreach (var item in projectService.Environments)
                {
                    var parentEnvironment = project.GetEnvironments().First(x => x.ProjectEnvironmentId == item.ProjectEnvironmentId);

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

                    if(item.Variables != null)
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
                @event.CPSType = project.OrganizationCPS.Type;
                @event.CPSAccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId);
                @event.CPSAccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName);
                @event.CPSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret);
                @event.CPSAccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion);

                await _eventBusService.Publish(queueName: "ProjectEnvironmentCreatedEvent", @event: @event);
            }
        }

        public async Task DeleteProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
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
                await _domainManagerService.AddForbidden($"You are not authorized to delete environments in this project.");
                return;
            }

            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to delete the environment.");
                return;
            }

            DomainModels.ProjectEnvironment environment = project.GetEnvironmentById(environmentId);
            if (environment == null)
            {
                await _domainManagerService.AddNotFound($"The environment with id {environmentId} does not exists.");
                return;
            };

            if (environment.Type == DomainModels.EnvironmentType.Root)
            {
                await _domainManagerService.AddConflict($"The environment root (Development) cannot be deleted.");
                return;
            };

            if (environment.Type == DomainModels.EnvironmentType.Fact)
            {
                await _domainManagerService.AddConflict($"The environment fact (Production) cannot be deleted.");
                return;
            };

            if (!(environment.Status == DomainModels.EntityStatus.Active || environment.Status == DomainModels.EntityStatus.Inactive))
            {
                await _domainManagerService.AddConflict($"The environment {environment.Name} must be in status Active/Inactive to be deleted.");
                return;
            }

            user.DeleteProjectEnvironment(organizationId, projectId, environmentId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            var projectServices = project.GetServicesWithReleaseStages();

            foreach (var projectService in projectServices)
            {
                var @event = new ProjectEnvironmentDeletedEvent(_correlationId)
                {
                    OrganizationId = organization.OrganizationId,
                    OrganizationName = organization.Name,
                    ProjectId = project.ProjectId,
                    ProjectExternalId = project.ProjectExternalId,
                    ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                    ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                    ProjectName = project.Name,
                    ServiceName = projectService.Name,
                    CMSType = project.OrganizationCMS.Type,
                    CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                    CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                    CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                    CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                    CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken),
                    EnvironmentName = environment.Name,
                    ReleseStageId = projectService.ReleaseStageId.Value
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

                await _eventBusService.Publish(queueName: "ProjectEnvironmentDeletedEvent", @event: @event);
            }
        }

        public async Task ActivateProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
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
                await _domainManagerService.AddForbidden($"You are not authorized to delete environments in this project.");
                return;
            }

            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to reactivate a project environment.");
                return;
            }

            DomainModels.ProjectEnvironment environment = project.GetEnvironmentById(environmentId);
            if (environment == null)
            {
                await _domainManagerService.AddNotFound($"The environment with id {environmentId} does not exists.");
                return;
            };

            if (environment.Type == DomainModels.EnvironmentType.Fact)
            {
                await _domainManagerService.AddConflict($"The environment fact cannot be reactivated.");
                return;
            };

            if (environment.Status != DomainModels.EntityStatus.Inactive)
            {
                await _domainManagerService.AddConflict($"The environment {environment.Name} must be in status Inactive to be reactivated.");
                return;
            }

            user.ReactivateProjectEnvironment(organizationId, projectId, environmentId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            var projectServices = project.GetServicesWithReleaseStages();

            foreach (var projectService in projectServices)
            {
                var @event = new ProjectEnvironmentActivatedEvent(_correlationId)
                {
                    OrganizationId = organization.OrganizationId,
                    OrganizationName = organization.Name,
                    ProjectId = project.ProjectId,
                    ProjectExternalId = project.ProjectExternalId,
                    ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                    ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                    ProjectName = project.Name,
                    ServiceName = projectService.Name,
                    CMSType = project.OrganizationCMS.Type,
                    CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                    CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                    CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                    CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                    CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken),
                    EnvironmentName = environment.Name,
                    ReleseStageId = projectService.ReleaseStageId.Value
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

                await _eventBusService.Publish(queueName: "ProjectEnvironmentActivatedEvent", @event: @event);
            }
            
        }

        public async Task InactivateProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
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
                await _domainManagerService.AddForbidden($"You are not authorized to delete environments in this project.");
                return;
            }

            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to inactivate a project environment.");
                return;
            }

            DomainModels.ProjectEnvironment environment = project.GetEnvironmentById(environmentId);
            if (environment == null)
            {
                await _domainManagerService.AddNotFound($"The environment with id {environmentId} does not exists.");
                return;
            };

            if (environment.Type == DomainModels.EnvironmentType.Fact)
            {
                await _domainManagerService.AddConflict($"The environment fact cannot be inactivated.");
                return;
            };

            if (environment.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The environment {environment.Name} must be in status Active to be inactivated.");
                return;
            }

            user.InactivateProjectEnvironment(organizationId, projectId, environmentId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            var projectServices = project.GetServicesWithReleaseStages();

            foreach (var projectService in projectServices)
            {
                var @event = new ProjectEnvironmentInactivatedEvent(_correlationId)
                {
                    OrganizationId = organization.OrganizationId,
                    OrganizationName = organization.Name,
                    ProjectId = project.ProjectId,
                    ProjectExternalId = project.ProjectExternalId,
                    ProjectExternalEndpointId = project.ProjectExternalEndpointId,
                    ProjectVSTSFakeName = project.ProjectVSTSFakeName,
                    ProjectName = project.Name,
                    ServiceName = projectService.Name,
                    CMSType = project.OrganizationCMS.Type,
                    CMSAccountId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountId),
                    CMSAccountName = _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName),
                    CMSAccessId = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessId),
                    CMSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret),
                    CMSAccessToken = _dataProtectorService.Unprotect(project.OrganizationCMS.AccessToken),
                    EnvironmentName = environment.Name,
                    ReleseStageId = projectService.ReleaseStageId.Value
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

                await _eventBusService.Publish(queueName: "ProjectEnvironmentInactivatedEvent", @event: @event);
            }
        }

        public async Task CreateReleaseProjectEnvironment(Guid organizationId, Guid projectId, Guid environmentId)
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

            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to create a release in an environment.");
                return;
            }

            DomainModels.ProjectEnvironment environment = project.GetEnvironmentById(environmentId);
            if (environment == null)
            {
                await _domainManagerService.AddNotFound($"The environment with id {environmentId} does not exists.");
                return;
            };
            
            var projectServices = project.GetServicesWithReleaseStages();

            var environmentsToBeSkippedList = project.Environments.Where(x => x.Rank < environment.Rank);
            var descriptionsToBeSkipped = $"Release created from PipelineSpace.";
            if (environmentsToBeSkippedList.Any())
            {
                descriptionsToBeSkipped = $"{descriptionsToBeSkipped} Detail: {string.Join(", ", environmentsToBeSkippedList.Select(x => $"PS_SKIP_ENVIRONMENT_{x.Name}"))}";
            }

            Parallel.ForEach(projectServices, async (service) => 
            {
                var previousEnvironment = service.Environments.FirstOrDefault(x => x.ProjectEnvironment.Rank == ((environment.Type == EnvironmentType.Root) ? environment.Rank : environment.Rank - 1));
                if (!string.IsNullOrEmpty(previousEnvironment.LastSuccessVersionId))
                {
                    CMSPipelineReleaseParamModel releaseBuildOptions = new CMSPipelineReleaseParamModel();
                    releaseBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
                    releaseBuildOptions.VSTSAccountName = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName) : _fakeAccountOptions.Value.AccountId;
                    releaseBuildOptions.VSTSAccessSecret = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret) : _fakeAccountOptions.Value.AccessSecret;
                    releaseBuildOptions.VSTSAccountProjectId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.Name : project.ProjectVSTSFakeName;

                    releaseBuildOptions.ProjectName = project.Name;
                    releaseBuildOptions.ProjectExternalId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.ProjectExternalId : project.ProjectVSTSFakeId;
                    releaseBuildOptions.ReleaseDefinitionId = service.ReleaseStageId.Value;
                    releaseBuildOptions.Alias = service.Name;

                    releaseBuildOptions.VersionId = int.Parse(previousEnvironment.LastSuccessVersionId);
                    releaseBuildOptions.VersionName = previousEnvironment.LastSuccessVersionName;
                    releaseBuildOptions.Description = descriptionsToBeSkipped;

                    await _cmsPipelineService.CreateRelease(releaseBuildOptions);
                }
            });
        }

        public async Task SortProjectEnvironments(Guid organizationId, Guid projectId, ProjectEnvironmentSortPostRp resource)
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
                await _domainManagerService.AddForbidden($"You are not authorized to sort environments in this project.");
                return;
            }
            
            if (project.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to sort de environments.");
                return;
            }

            var developmentEnvironment = project.GetDevelopmentEnvironment();
            var productionEnvironment = project.GetProductionEnvironment();

            foreach (var item in resource.Items)
            {
                var environment = project.GetEnvironmentById(item.EnvironmentId);

                if(environment == null)
                {
                    await _domainManagerService.AddNotFound($"The environment with id {item.EnvironmentId}) does not exists.");
                    return;
                }

                if(environment.Type == EnvironmentType.Root || environment.Type == EnvironmentType.Fact)
                {
                    await _domainManagerService.AddConflict($"The environment {environment.Name} ({environment.ProjectEnvironmentId}) is not sortable.");
                    return;
                }

                if (!(developmentEnvironment.Rank < item.Rank && item.Rank < productionEnvironment.Rank))
                {
                    await _domainManagerService.AddConflict($"The rank of the environment {environment.Name} ({environment.ProjectEnvironmentId}) must be between {developmentEnvironment.Rank + 1} and {productionEnvironment.Rank - 1}.");
                    return;
                }

                environment.Rank = item.Rank;
            }

            var groupped = project.Environments.GroupBy(x => x.Rank);
            if (groupped.Count() != project.Environments.Count)
            {
                await _domainManagerService.AddConflict($"The rank of the environments must be sorted sequentially between {developmentEnvironment.Rank + 1} and {productionEnvironment.Rank - 1}.");
                return;
            }

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            var projectServices = project.GetServicesWithReleaseStages();

            //send events
            foreach (var projectService in projectServices)
            {
                var @event = new ProjectEnvironmentCreatedEvent(_correlationId)
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
                    ReleseStageId = projectService.ReleaseStageId.Value,
                    ServiceName = projectService.Name,
                    ServiceLastBuildSuccessVersionId = projectService.LastBuildSuccessVersionId,
                    ServiceLastBuildSuccessVersionName = projectService.LastBuildSuccessVersionName
                };

                @event.Environments = new List<ProjectEnvironmentItemCreatedEvent>();

                foreach (var item in projectService.Environments)
                {
                    var parentEnvironment = project.GetEnvironments().First(x => x.ProjectEnvironmentId == item.ProjectEnvironmentId);

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
                @event.CPSType = project.OrganizationCPS.Type;
                @event.CPSAccessId = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessId);
                @event.CPSAccessName = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessName);
                @event.CPSAccessSecret = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessSecret);
                @event.CPSAccessRegion = _dataProtectorService.Unprotect(project.OrganizationCPS.AccessRegion);

                await _eventBusService.Publish(queueName: "ProjectEnvironmentCreatedEvent", @event: @event);
            }
        }
    }
}
