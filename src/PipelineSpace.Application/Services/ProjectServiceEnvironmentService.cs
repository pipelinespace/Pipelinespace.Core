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
using Microsoft.Extensions.Options;
using PipelineSpace.Infra.Options;
using PipelineSpace.Domain.Models;

namespace PipelineSpace.Application.Services
{
    public class ProjectServiceEnvironmentService : IProjectServiceEnvironmentService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IUserRepository _userRepository;
        readonly Func<DomainModels.CloudProviderService, ICPSQueryService> _cpsQueryService;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly ICMSPipelineService _cmsPipelineService;
        readonly IOptions<VSTSServiceOptions> _vstsOptions;
        readonly IOptions<FakeAccountServiceOptions> _fakeAccountOptions;
        readonly IDataProtectorService _dataProtectorService;

        public ProjectServiceEnvironmentService(IDomainManagerService domainManagerService,
                                                     IIdentityService identityService,
                                                     IUserRepository userRepository,
                                                     Func<DomainModels.CloudProviderService, ICPSQueryService> cpsQueryService,
                                                     IEventBusService eventBusService,
                                                     IActivityMonitorService activityMonitorService,
                                                     ICMSPipelineService cmsPipelineService,
                                                     IOptions<VSTSServiceOptions> vstsOptions,
                                                     IOptions<FakeAccountServiceOptions> fakeAccountOptions,
                                                     IDataProtectorService dataProtectorService
                                                     )
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _userRepository = userRepository;
            _cpsQueryService = cpsQueryService;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _cmsPipelineService = cmsPipelineService;
            _vstsOptions = vstsOptions;
            _fakeAccountOptions = fakeAccountOptions;
            _dataProtectorService = dataProtectorService;
        }

        public async Task CreateProjectServiceEnvironmentVariables(Guid organizationId, Guid projectId, Guid serviceId, Guid environmentId, ProjectServiceEnvironmentVariablePostRp resource)
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

            DomainModels.ProjectService projectService = project.GetServiceById(serviceId);
            if (projectService == null)
            {
                await _domainManagerService.AddNotFound($"The project service with id {serviceId} does not exists.");
                return;
            }

            if (projectService.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The pipe with id {serviceId} must be in status Active to add/modify variables.");
                return;
            }

            DomainModels.ProjectServiceEnvironment environment = projectService.GetServiceEnvironment(environmentId);
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
                ReleseStageId = projectService.ReleaseStageId.Value
            };

            @event.Environments = new List<ProjectEnvironmentItemCreatedEvent>();

            foreach (var item in projectService.Environments)
            {
                var parentEnvironment = project.GetEnvironments().First(x => x.ProjectEnvironmentId == item.ProjectEnvironmentId);

                var serviceEnvironment = new ProjectEnvironmentItemCreatedEvent();
                serviceEnvironment.Name = parentEnvironment.Name;
                serviceEnvironment.RequiredApproval = parentEnvironment.RequiresApproval;
                serviceEnvironment.Rank = parentEnvironment.Rank;

                serviceEnvironment.Variables = new List<ProjectEnvironmentItemVariableCreatedEvent>();
                foreach (var variable in parentEnvironment.Variables)
                {
                    serviceEnvironment.Variables.Add(new ProjectEnvironmentItemVariableCreatedEvent()
                    {
                        Name = variable.Name,
                        Value = variable.Value
                    });
                }
                foreach (var variable in item.Variables)
                {
                    serviceEnvironment.Variables.Add(new ProjectEnvironmentItemVariableCreatedEvent()
                    {
                        Name = variable.Name,
                        Value = variable.Value
                    });
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

        public async Task CreateProjectServiceEnvironmentRelease(Guid organizationId, Guid projectId, Guid serviceId, Guid environmentId)
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
                await _domainManagerService.AddConflict($"The project with id {projectId} must be in status Active to add a new feature.");
                return;
            }

            DomainModels.ProjectService projectService = project.GetServiceById(serviceId);
            if (projectService == null)
            {
                await _domainManagerService.AddNotFound($"The project service with id {serviceId} does not exists.");
                return;
            }

            if (projectService.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The pipe with id {serviceId} must be in status Active to create a release.");
                return;
            }

            DomainModels.ProjectServiceEnvironment environment = projectService.GetServiceEnvironment(environmentId);
            if (environment == null)
            {
                await _domainManagerService.AddNotFound($"The environment with id {environmentId} does not exists.");
                return;
            }

            if (environment.Status != DomainModels.EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The environment with id {environmentId} must be in status Active to create a release.");
                return;
            }

            var previousEnvironment = projectService.Environments.FirstOrDefault(x => x.ProjectEnvironment.Rank == ((environment.ProjectEnvironment.Type == EnvironmentType.Root) ? environment.ProjectEnvironment.Rank : environment.ProjectEnvironment.Rank - 1));
            if (string.IsNullOrEmpty(previousEnvironment.LastSuccessVersionId))
            {
                await _domainManagerService.AddConflict($"The project service with id {serviceId} does not have any success deploy in the environment {previousEnvironment.ProjectEnvironment.Name} (This is a condition to create the release).");
                return;
            }

            var environmentsToBeSkippedList = project.Environments.Where(x => x.Rank < environment.ProjectEnvironment.Rank);
            var descriptionsToBeSkipped = $"Release created from PipelineSpace.";
            if (environmentsToBeSkippedList.Any())
            {
                descriptionsToBeSkipped = $"{descriptionsToBeSkipped} Detail: {string.Join(", ", environmentsToBeSkippedList.Select(x => $"PS_SKIP_ENVIRONMENT_{x.Name}"))}";
            }

            CMSPipelineReleaseParamModel releaseBuildOptions = new CMSPipelineReleaseParamModel();
            releaseBuildOptions.VSTSAPIVersion = _vstsOptions.Value.ApiVersion;
            releaseBuildOptions.VSTSAccountName = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(project.OrganizationCMS.AccountName) : _fakeAccountOptions.Value.AccountId;
            releaseBuildOptions.VSTSAccessSecret = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? _dataProtectorService.Unprotect(project.OrganizationCMS.AccessSecret) : _fakeAccountOptions.Value.AccessSecret;
            releaseBuildOptions.VSTSAccountProjectId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.Name : project.ProjectVSTSFakeName;

            releaseBuildOptions.ProjectName = project.Name;
            releaseBuildOptions.ProjectExternalId = project.OrganizationCMS.Type == ConfigurationManagementService.VSTS ? project.ProjectExternalId : project.ProjectVSTSFakeId;
            releaseBuildOptions.ReleaseDefinitionId = projectService.ReleaseStageId.Value;
            releaseBuildOptions.Alias = projectService.Name;
            
            releaseBuildOptions.VersionId = int.Parse(previousEnvironment.LastSuccessVersionId);
            releaseBuildOptions.VersionName = previousEnvironment.LastSuccessVersionName;
            releaseBuildOptions.Description = descriptionsToBeSkipped;

            await _cmsPipelineService.CreateRelease(releaseBuildOptions);

        }
    }
}
