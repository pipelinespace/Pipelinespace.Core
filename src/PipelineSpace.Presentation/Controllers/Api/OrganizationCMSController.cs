using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Models;
using PipelineSpace.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin,organizationadmin")]
    [Route("{api}/organizations")]
    public class OrganizationCMSController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IOrganizationCMSService _organizationCMSService;
        readonly IOrganizationCMSQueryService _organizationCMSQueryService;

        public OrganizationCMSController(IDomainManagerService domainManagerService,
                                         IOrganizationCMSService organizationCMSService,
                                         IOrganizationCMSQueryService organizationCMSQueryService) : base (domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _organizationCMSService = organizationCMSService;
            _organizationCMSQueryService = organizationCMSQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/configurationmanagementservices")]
        public async Task<IActionResult> GetOrganizationConfigurationManagementServices(Guid organizationId, [FromQuery(Name = "type")]CMSConnectionType connectionType)
        {
            var configurationManagementServices = await _organizationCMSQueryService.GetOrganizationConfigurationManagementServices(organizationId, connectionType);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(configurationManagementServices);
        }

        [HttpGet]
        [Route("{organizationId:guid}/configurationmanagementservices/{organizationCMSId:guid}")]
        public async Task<IActionResult> GetOrganizationConfigurationManagementServiceById(Guid organizationId, Guid organizationCMSId)
        {
            var configurationManagementService = await _organizationCMSQueryService.GetOrganizationConfigurationManagementServiceById(organizationId, organizationCMSId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (configurationManagementService == null)
                return this.NotFound();

            return this.Ok(configurationManagementService);
        }

        [HttpPost]
        [Route("{organizationId:guid}/configurationmanagementservices")]
        public async Task<IActionResult> CreateOrganizationConfigurationManagement(Guid organizationId, [FromBody]OrganizationCMSPostRp organizationCMSRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            if ((organizationCMSRp.Type == ConfigurationManagementService.VSTS ||
                 organizationCMSRp.Type == ConfigurationManagementService.Bitbucket) && string.IsNullOrEmpty(organizationCMSRp.AccessSecret))
            {
                if (organizationCMSRp.Type == ConfigurationManagementService.VSTS)
                    ModelState.AddModelError("", "Personal Access Token is required");

                if (organizationCMSRp.Type == ConfigurationManagementService.Bitbucket)
                    ModelState.AddModelError("", "App Password is required");

                return this.BadRequest(ModelState);
            }

            await _organizationCMSService.CreateConfigurationManagementService(organizationId, organizationCMSRp);

            if (_domainManagerService.HasNotFounds())
            {
                return this.Forbidden(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("{organizationId:guid}/configurationmanagementservices/{organizationCMSId:guid}")]
        public async Task<IActionResult> UpdateConfigurationManagementService(Guid organizationId, Guid organizationCMSId, [FromBody]OrganizationCMSPutRp organizationCMSRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationCMSService.UpdateConfigurationManagementService(organizationId, organizationCMSId, organizationCMSRp);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }

        [HttpDelete]
        [Route("{organizationId:guid}/configurationmanagementservices/{organizationCMSId:guid}")]
        public async Task<IActionResult> DeleteConfigurationManagementService(Guid organizationId, Guid organizationCMSId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationCMSService.DeleteConfigurationManagementService(organizationId, organizationCMSId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("{organizationId:guid}/configurationmanagementservices/{organizationCMSId:guid}/agentpools")]
        public async Task<IActionResult> GetOrganizationConfigurationManagementServiceAgentPools(Guid organizationId, Guid organizationCMSId)
        {
            var agentPools = await _organizationCMSQueryService.GetOrganizationConfigurationManagementServiceAgentPools(organizationId, organizationCMSId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (agentPools == null)
                return this.NotFound();

            return this.Ok(agentPools);
        }

        [HttpGet]
        [Route("{organizationId:guid}/configurationmanagementservices/{organizationCMSId:guid}/teams")]
        public async Task<IActionResult> GetOrganizationConfigurationManagementServiceTeams(Guid organizationId, Guid organizationCMSId)
        {
            var projects = await _organizationCMSQueryService.GetOrganizationConfigurationManagementServiceTeams(organizationId, organizationCMSId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (projects == null)
                return this.NotFound();

            return this.Ok(projects);
        }


        [HttpGet]
        [Route("{organizationId:guid}/configurationmanagementservices/{organizationCMSId:guid}/projects")]
        public async Task<IActionResult> GetOrganizationConfigurationManagementServiceProjects(Guid organizationId, Guid organizationCMSId)
        {
            var projects = await _organizationCMSQueryService.GetOrganizationConfigurationManagementServiceProjects(organizationId, organizationCMSId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (projects == null)
                return this.NotFound();

            return this.Ok(projects);
        }

        [HttpGet]
        [Route("{organizationId:guid}/configurationmanagementservices/{organizationCMSId:guid}/repositories")]
        public async Task<IActionResult> GetOrganizationConfigurationManagementServiceRepositories(Guid organizationId, Guid organizationCMSId, [FromQuery(Name = "projectId")]string projectId)
        {
            var respositories = await _organizationCMSQueryService.GetOrganizationConfigurationManagementServiceRepositories(organizationId, organizationCMSId, projectId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (respositories == null)
                return this.NotFound();

            return this.Ok(respositories);
        }

        [HttpGet]
        [Route("{organizationId:guid}/configurationmanagementservices/{organizationCMSId:guid}/repositories/{repositoryId}/branches")]
        public async Task<IActionResult> GetOrganizationConfigurationManagementServiceRepositoriesBranches(Guid organizationId, Guid organizationCMSId, 
            string repositoryId,
            [FromQuery(Name = "projectId")]string projectId)
        {
            var respositories = await _organizationCMSQueryService.GetOrganizationConfigurationManagementServiceRepositoriesBranches(organizationId, 
                organizationCMSId, projectId, repositoryId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (respositories == null)
                return this.NotFound();

            return this.Ok(respositories);
        }

    }
}
