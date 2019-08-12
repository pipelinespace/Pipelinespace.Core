using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.AggregationApi
{
    [Authorize(Roles = "globaladmin,organizationadmin")]
    [Route("{api}/aggregations")]
    public class GatewayController : BaseController
    {
        readonly IOrganizationQueryService _organizationQueryService;
        readonly IProjectQueryService _projectQueryService;
        readonly IOrganizationCMSQueryService _organizationCMSQueryService;
        readonly IOrganizationCPSQueryService _organizationCPSQueryService;
        readonly IOrganizationUserQueryService _organizationUserQueryService;
        readonly IProjectActivityQueryService _projectActivityQueryService;
        readonly IProjectEnvironmentQueryService _projectEnvironmentQueryService;
        readonly IProjectFeatureQueryService _projectFeatureQueryService;
        readonly IProjectServiceQueryService _projectServiceQueryService;
        readonly IProjectUserQueryService _projectUserQueryService;
        readonly IProjectUserInvitationQueryService _projectUserInvitationQueryService;
        readonly IOrganizationUserInvitationQueryService _organizationUserInvitationQueryService;
        readonly IProjectServiceEventQueryService _projectServiceEventQueryService;
        readonly IProjectServiceEnvironmentQueryService _projectServiceEnvironmentQueryService;
        readonly IProjectServiceActivityQueryService _projectServiceActivityQueryService;
        readonly IProjectServiceCloudQueryService _projectServiceCloudQueryService;
        readonly IProjectFeatureServiceActivityQueryService _projectFeatureServiceActivityQueryService;
        readonly IProjectFeatureServiceCloudQueryService _projectFeatureServiceCloudQueryService;
        readonly IProjectFeatureServiceQueryService _projectFeatureServiceQueryService;
        readonly IProjectFeatureServiceEnvironmentQueryService _projectFeatureServiceEnvironmentQueryService;
        readonly IProjectFeatureServiceEventQueryService _projectFeatureServiceEventQueryService;

        public GatewayController(IDomainManagerService domainManagerService,
            IOrganizationQueryService organizationQueryService,
            IProjectQueryService projectQueryService,
            IOrganizationCMSQueryService organizationCMSQueryService,
            IOrganizationCPSQueryService organizationCPSQueryService,
            IOrganizationUserQueryService organizationUserQueryService,
            IProjectActivityQueryService projectActivityQueryService,
            IProjectEnvironmentQueryService projectEnvironmentQueryService,
            IProjectFeatureQueryService projectFeatureQueryService,
            IProjectServiceQueryService projectServiceQueryService,
            IProjectUserQueryService projectUserQueryService,
            IProjectUserInvitationQueryService projectUserInvitationQueryService,
            IOrganizationUserInvitationQueryService organizationUserInvitationQueryService,
            IProjectServiceEventQueryService projectServiceEventQueryService,
            IProjectServiceEnvironmentQueryService projectServiceEnvironmentQueryService,
            IProjectServiceActivityQueryService projectServiceActivityQueryService,
            IProjectServiceCloudQueryService projectServiceCloudQueryService,
            IProjectFeatureServiceActivityQueryService projectFeatureServiceActivityQueryService,
            IProjectFeatureServiceCloudQueryService projectFeatureServiceCloudQueryService,
            IProjectFeatureServiceQueryService projectFeatureServiceQueryService,
            IProjectFeatureServiceEventQueryService projectFeatureServiceEventQueryService,
            IProjectFeatureServiceEnvironmentQueryService projectFeatureServiceEnvironmentQueryService
            ) : base(domainManagerService)
        {
            this._organizationQueryService = organizationQueryService;
            this._projectQueryService = projectQueryService;
            this._organizationCMSQueryService = organizationCMSQueryService;
            this._organizationCPSQueryService = organizationCPSQueryService;
            this._organizationUserQueryService = organizationUserQueryService;
            this._projectActivityQueryService = projectActivityQueryService;
            this._projectEnvironmentQueryService = projectEnvironmentQueryService;
            this._projectFeatureQueryService = projectFeatureQueryService;
            this._projectServiceQueryService = projectServiceQueryService;
            this._projectUserQueryService = projectUserQueryService;
            this._projectUserInvitationQueryService = projectUserInvitationQueryService;
            this._organizationUserInvitationQueryService = organizationUserInvitationQueryService;
            this._projectServiceEventQueryService = projectServiceEventQueryService;
            this._projectServiceEnvironmentQueryService = projectServiceEnvironmentQueryService;
            this._projectServiceActivityQueryService = projectServiceActivityQueryService;
            this._projectServiceCloudQueryService = projectServiceCloudQueryService;
            this._projectFeatureServiceActivityQueryService = projectFeatureServiceActivityQueryService;
            this._projectFeatureServiceCloudQueryService = projectFeatureServiceCloudQueryService;
            this._projectFeatureServiceEventQueryService = projectFeatureServiceEventQueryService;
            this._projectFeatureServiceQueryService = projectFeatureServiceQueryService;
            this._projectFeatureServiceEnvironmentQueryService = projectFeatureServiceEnvironmentQueryService;
        }

        [HttpGet]
        [Route("organizations/{organizationId:guid}")]
        public async Task<IActionResult> GetOrganizationById(Guid organizationId)
        {
            var organization = await _organizationQueryService.GetOrganizationById(organizationId);

            if (organization == null)
                return this.NotFound();

            var projects = await this._projectQueryService.GetProjectsWithServices(organizationId);
            var configurationManagementServices = await _organizationCMSQueryService.GetOrganizationConfigurationManagementServices(organizationId, Domain.Models.Enums.CMSConnectionType.ProjectLevel);
            var cloudProviderServices = await _organizationCPSQueryService.GetOrganizationCloudProviderServices(organizationId);
            var users = await _organizationUserQueryService.GetUsers(organizationId);
            var invitations = await _organizationUserInvitationQueryService.GetInvitations(organizationId);

            var model = new {
                organization = organization,
                projects = projects,
                cms = configurationManagementServices,
                cps = cloudProviderServices,
                users = users,
                invitations = invitations
            };

            return this.Ok(model);
        }

        [HttpGet]
        [Route("organizations/{organizationId:guid}/projects/{projectId:guid}")]
        public async Task<IActionResult> GetProjectById(Guid organizationId, Guid projectId)
        {
            var organization = await _organizationQueryService.GetOrganizationById(organizationId);

            if (organization == null)
                return this.NotFound();

            var project = await _projectQueryService.GetProjectById(organizationId, projectId);

            if (project == null)
                return this.NotFound();

            var activities = await _projectActivityQueryService.GetProjectActivities(organizationId, projectId);
            var environments = await _projectEnvironmentQueryService.GetProjectEnvironments(organizationId, projectId);
            var features = await _projectFeatureQueryService.GetProjectFeatures(organizationId, projectId);
            var services = await _projectServiceQueryService.GetProjectServices(organizationId, projectId);
            var users = await _projectUserQueryService.GetUsers(organizationId, projectId);
            var invitations = await _projectUserInvitationQueryService.GetInvitations(organizationId, projectId);
            var projectProvider = await _projectQueryService.GetProjectExternalById(organizationId, projectId);

            var model = new
            {
                organization = organization,
                project = project,
                projectProvider = projectProvider,
                activities = activities,
                environments = environments,
                features = features,
                services = services,
                users = users,
                invitations = invitations
            };

            return this.Ok(model);
        }

        [HttpGet]
        [Route("organizations/{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}")]
        public async Task<IActionResult> GetServiceById(Guid organizationId, Guid projectId, Guid serviceId)
        {
            var organization = await _organizationQueryService.GetOrganizationById(organizationId);

            if (organization == null)
                return this.NotFound();

            var project = await _projectQueryService.GetProjectById(organizationId, projectId);

            if (project == null)
                return this.NotFound();

            var service = await _projectServiceQueryService.GetProjectServiceById(organizationId, projectId, serviceId);

            if (service == null)
                return this.NotFound();

            var activities = await _projectServiceActivityQueryService.GetProjectServiceActivities(organizationId, projectId, serviceId);
            var events = await _projectServiceEventQueryService.GetProjectServiceEvents(organizationId, projectId, serviceId, BaseEventType.None);
            var builds = await _projectServiceEventQueryService.GetProjectServiceEvents(organizationId, projectId, serviceId, BaseEventType.Build);
            var releases = await _projectServiceEventQueryService.GetProjectServiceEvents(organizationId, projectId, serviceId, BaseEventType.Release);
            var environments = await _projectServiceEnvironmentQueryService.GetProjectServiceEnvironments(organizationId, projectId, serviceId);
            var pipeline = await _projectServiceQueryService.GetProjectServicePipelineById(organizationId, projectId, serviceId);
            var features = await _projectServiceQueryService.GetProjectServiceFeaturesById(organizationId, projectId, serviceId);
            var serviceProvider = await _projectServiceQueryService.GetProjectServiceExternalById(organizationId, projectId, serviceId);

            var model = new
            {
                organization = organization,
                project = project,
                service = service,
                activities = activities,
                events = events,
                builds = builds,
                releases = releases,
                environments = environments,
                pipeline = pipeline,
                features = features,
                serviceProvider = serviceProvider
            };

            return this.Ok(model);
        }

        [HttpGet]
        [Route("organizations/{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}")]
        public async Task<IActionResult> GetFeatureServiceById(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId)
        {
            var organization = await _organizationQueryService.GetOrganizationById(organizationId);

            if (organization == null)
                return this.NotFound();

            var project = await _projectQueryService.GetProjectById(organizationId, projectId);

            if (project == null)
                return this.NotFound();

            var feature = await _projectFeatureQueryService.GetProjectFeatureById(organizationId, projectId, featureId);

            if (feature == null)
                return this.NotFound();

            var service = await _projectServiceQueryService.GetProjectServiceById(organizationId, projectId, serviceId);

            if (service == null)
                return this.NotFound();
            

            var activities = await _projectFeatureServiceActivityQueryService.GetProjectFeatureServiceActivities(organizationId, projectId, featureId, serviceId);
            var events = await _projectFeatureServiceEventQueryService.GetProjectFeatureServiceEvents(organizationId, projectId, featureId, serviceId, BaseEventType.None);
            var builds = await _projectFeatureServiceEventQueryService.GetProjectFeatureServiceEvents(organizationId, projectId, featureId, serviceId, BaseEventType.Build);
            var releases = await _projectFeatureServiceEventQueryService.GetProjectFeatureServiceEvents(organizationId, projectId, featureId, serviceId, BaseEventType.Release);
            var environments = await _projectFeatureServiceEnvironmentQueryService.GetFeatureProjectServiceEnvironments(organizationId, projectId, featureId, serviceId);
            var pipeline = await _projectFeatureServiceQueryService.GetProjectFeatureServicePipelineById(organizationId, projectId, featureId, serviceId);
            var serviceProvider = await _projectFeatureServiceQueryService.GetProjectFeatureServiceExternalById(organizationId, projectId, featureId,  serviceId);

            var model = new
            {
                organization = organization,
                project = project,
                service = service,
                feature = feature,
                activities = activities,
                events = events,
                builds = builds,
                releases = releases,
                environments = environments,
                pipeline = pipeline,
                serviceProvider = serviceProvider
            };

            return this.Ok(model);
        }

        [HttpGet]
        [Route("organizations/{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}")]
        public async Task<IActionResult> GetFeatureById(Guid organizationId, Guid projectId, Guid featureId)
        {
            var organization = await _organizationQueryService.GetOrganizationById(organizationId);

            if (organization == null)
                return this.NotFound();

            var project = await _projectQueryService.GetProjectById(organizationId, projectId);

            if (project == null)
                return this.NotFound();

            var feature = await _projectFeatureQueryService.GetProjectFeatureById(organizationId, projectId, featureId);

            if (feature == null)
                return this.NotFound();
            
            var services = await _projectFeatureServiceQueryService.GetProjectFeatureServices(organizationId, projectId, featureId);
            
            var model = new
            {
                organization = organization,
                project = project,
                feature = feature,
                services = services,
            };

            return this.Ok(model);
        }
    }
}
