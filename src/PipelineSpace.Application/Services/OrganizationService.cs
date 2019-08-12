using PipelineSpace.Application.Interfaces;
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
    public class OrganizationService : IOrganizationService
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IIdentityService _identityService;
        readonly IOrganizationRepository _organizationRepository;
        readonly IProjectRepository _projectRepository;
        readonly IProjectFeatureRepository _projectFeatureRepository;
        readonly IOrganizationCPSRepository _organizationCPSRepository;
        readonly IOrganizationCMSRepository _organizationCMSRepository;
        readonly IUserRepository _userRepository;
        readonly IEventBusService _eventBusService;
        readonly string _correlationId;
        readonly IDataProtectorService _dataProtectorService;

        public OrganizationService(IDomainManagerService domainManagerService,
                                   IIdentityService identityService,
                                   IOrganizationRepository organizationRepository,
                                   IProjectRepository projectRepository,
                                   IProjectFeatureRepository projectFeatureRepository,
                                   IOrganizationCPSRepository organizationCPSRepository,
                                   IOrganizationCMSRepository organizationCMSRepository,
                                   IUserRepository userRepository,
                                   IEventBusService eventBusService,
                                   IActivityMonitorService activityMonitorService,
                                   IDataProtectorService dataProtectorService)
        {
            _domainManagerService = domainManagerService;
            _identityService = identityService;
            _organizationRepository = organizationRepository;
            _projectRepository = projectRepository;
            _projectFeatureRepository = projectFeatureRepository;
            _organizationCPSRepository = organizationCPSRepository;
            _organizationCMSRepository = organizationCMSRepository;
            _userRepository = userRepository;
            _eventBusService = eventBusService;
            _correlationId = activityMonitorService.GetCorrelationId();
            _dataProtectorService = dataProtectorService;
        }

        public async Task CreateOrganization(OrganizationPostRp resource)
        {
            string ownerUserId = _identityService.GetOwnerId();
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);
            
            Organization existingOrganization = user.FindOrganizationByName(resource.Name);
            if (existingOrganization != null)
            {
                await _domainManagerService.AddConflict($"The organzation name {resource.Name} has already been taken.");
                return;
            }

            Organization newOrganization = user.CreateOrganization(resource.Name, resource.Description, resource.WebSiteUrl, ownerUserId);

            _userRepository.Update(user);
            
            await _userRepository.SaveChanges();

            await _domainManagerService.AddResult("OrganizationId", newOrganization.OrganizationId);
        }

        public async Task UpdateOrganization(Guid organizationId, OrganizationPutRp resource)
        {
            string ownerUserId = _identityService.GetOwnerId();
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation name {resource.Name} with id {organizationId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);
            if (role != PipelineRole.OrganizationAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to perform updates in this organization.");
                return;
            }

            Organization existingOrganization = user.FindOrganizationByName(resource.Name);
            if (existingOrganization != null && existingOrganization.OrganizationId != organizationId)
            {
                await _domainManagerService.AddConflict($"The organzation name {resource.Name} has already been taken.");
                return;
            }

            user.UpdateOrganization(organizationId, resource.Name, resource.Description, resource.WebSiteUrl);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }

        public async Task DeleteOrganization(Guid organizationId)
        {
            string loggedUserId = _identityService.GetUserId();

            User user = await _userRepository.GetUser(loggedUserId);

            Organization organization = user.FindOrganizationById(organizationId);
            if (organization == null)
            {
                await _domainManagerService.AddNotFound($"The organzation with id {organizationId} does not exists.");
                return;
            }

            PipelineRole role = user.GetRoleInOrganization(organizationId);
            if (role != PipelineRole.OrganizationAdmin)
            {
                await _domainManagerService.AddForbidden($"You are not authorized to delete this organization.");
                return;
            }

            if (organization.Status != EntityStatus.Active)
            {
                await _domainManagerService.AddConflict($"The organization with id {organizationId} must be in status Active to be modified/deleted.");
                return;
            }

            var preparingProjects = organization.GetPreparingProjects();
            if (preparingProjects.Any())
            {
                await _domainManagerService.AddConflict($"The organization with id {organizationId} has projects in status Preparing. All projects must be in status Active to delete the organization");
                return;
            }

            //validate if there are any projects/services/features in preparing status
            foreach (var project in organization.Projects)
            {
                var preparingServices = project.GetPreparingServices();
                if (preparingServices.Any())
                {
                    await _domainManagerService.AddConflict($"The project with id {project.ProjectId} ({project.Name}) has pipes in status Preparing. All services must be in status Active to delete the project");
                    return;
                }

                var preparingFeatures = project.GetPreparingFeatures();
                if (preparingFeatures.Any())
                {
                    await _domainManagerService.AddConflict($"The project with id {project.ProjectId} ({project.Name}) has features in status Preparing. All features must be in status Active to delete the project");
                    return;
                }
            }

            user.DeleteOrganization(organizationId);

            _userRepository.Update(user);

            await _userRepository.SaveChanges();

            //send event to delete projects in CMS

            var projects = await _organizationRepository.GetProjects(organizationId);

            List<ProjectDeletedEvent> projectEventList = new List<ProjectDeletedEvent>();
            foreach (var project in projects)
            {
                var organizationCMS = await _organizationCMSRepository.FindOrganizationCMSById(project.OrganizationCMSId);
                projectEventList.Add(new ProjectDeletedEvent(_correlationId, project.IsImported)
                {
                    OrganizationExternalId = project.OrganizationExternalId,
                    CMSType = organizationCMS.Type,
                    CMSAccountId = _dataProtectorService.Unprotect(organizationCMS.AccountId),
                    CMSAccountName = _dataProtectorService.Unprotect(organizationCMS.AccountName),
                    CMSAccessId = _dataProtectorService.Unprotect(organizationCMS.AccessId),
                    CMSAccessSecret = _dataProtectorService.Unprotect(organizationCMS.AccessSecret),
                    CMSAccessToken = _dataProtectorService.Unprotect(organizationCMS.AccessToken),
                    ProjectExternalId = project.ProjectExternalId,
                    ProjectVSTSFakeId = project.ProjectVSTSFakeId
                });
            }

            var organizationDeletedEvent = new OrganizationDeletedEvent(_correlationId)
            {
                Projects = projectEventList
            };

            await _eventBusService.Publish(queueName: "OrganizationDeletedEvent", @event: organizationDeletedEvent);
            
            foreach (var project in projects)
            {
                //send event to delete clous services in CPS - Services
                var services = await _projectRepository.GetProjectServices(organizationId, project.ProjectId);
                var environments = await _projectRepository.GetProjectEnvironments(organizationId, project.ProjectId);

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
                        SourceEvent = Domain.Models.Enums.SourceEvent.Organization
                    };

                    await _eventBusService.Publish(queueName: "ProjectServiceDeletedEvent", @event: projectServiceDeletedEvent);
                }

                //send event to delete clous services in CPS - Features
                var features = await _projectRepository.GetProjectFeatures(organizationId, project.ProjectId);
                foreach (var feature in features)
                {
                    var featureServices = await _projectFeatureRepository.GetProjectFeatureServices(organizationId, project.ProjectId, feature.ProjectFeatureId);

                    List<ProjectFeatureServiceDeletedEvent> projectFeatureServiceDeletedEventList = new List<ProjectFeatureServiceDeletedEvent>();
                    foreach (var item in feature.Services)
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
                        SourceEvent = Domain.Models.Enums.SourceEvent.Organization
                    };

                    await _eventBusService.Publish(queueName: "ProjectFeatureDeletedEvent", @event: projectFeatureDeletedEvent);
                }
            }
        }
    }
}
