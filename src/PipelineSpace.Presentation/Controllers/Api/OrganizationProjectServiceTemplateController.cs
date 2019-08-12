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

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin,organizationadmin")]
    [Route("{api}/organizations")]
    public class OrganizationProjectServiceTemplateController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IOrganizationProjectServiceTemplateService _organizationProjectServiceTemplateService;
        readonly IOrganizationProjectServiceTemplateQueryService _organizationProjectServiceTemplateQueryService;

        public OrganizationProjectServiceTemplateController(IDomainManagerService domainManagerService,
                                                            IOrganizationProjectServiceTemplateService organizationProjectServiceTemplateService,
                                                            IOrganizationProjectServiceTemplateQueryService organizationProjectServiceTemplateQueryService) : base (domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _organizationProjectServiceTemplateService = organizationProjectServiceTemplateService;
            _organizationProjectServiceTemplateQueryService = organizationProjectServiceTemplateQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/servicetemplates/all")]
        public async Task<IActionResult> GetAllOrganizationProjectServiceTemplates(Guid organizationId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            var templates = await _organizationProjectServiceTemplateQueryService.GetAllOrganizationProjectServiceTemplates(organizationId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok(templates);
        }

        [HttpGet]
        [Route("{organizationId:guid}/servicetemplates")]
        public async Task<IActionResult> GetOrganizationProjectServiceTemplates(Guid organizationId,
                                                                                [FromQuery(Name = "cloudProviderType")]CloudProviderService cloudProviderType,
                                                                                [FromQuery(Name = "pipeType")]PipeType pipeType)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            var templates = await _organizationProjectServiceTemplateQueryService.GetOrganizationProjectServiceTemplates(organizationId, cloudProviderType, pipeType);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok(templates);
        }

        [HttpPost]
        [Route("{organizationId:guid}/servicetemplates")]
        public async Task<IActionResult> CreateOrganizationProjectServiceTemplate(Guid organizationId, [FromBody]OrganizationProjectServiceTemplatePostRp resourceRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            if (resourceRp.RepositoryCMSType == ConfigurationManagementService.VSTS)
            {
                if (string.IsNullOrEmpty(resourceRp.ProjectExternalId))
                {
                    ModelState.AddModelError("", "Project External Id is required");
                }

                if (string.IsNullOrEmpty(resourceRp.ProjectExternalName))
                {
                    ModelState.AddModelError("", "Project External Name is required");
                }
            }

            if (ModelState.ErrorCount > 0)
            {
                return this.BadRequest(ModelState);
            }

            await _organizationProjectServiceTemplateService.CreateOrganizationProjectServiceTemplate(organizationId, resourceRp);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetForbidden());
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
        [Route("{organizationId:guid}/servicetemplates/{projectServiceTemplateId:guid}")]
        public async Task<IActionResult> UpdateOrganizationProjectServiceTemplate(Guid organizationId, Guid projectServiceTemplateId, [FromBody]OrganizationProjectServiceTemplatePutRp resourceRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);
            
            await _organizationProjectServiceTemplateService.UpdateOrganizationProjectServiceTemplate(organizationId, projectServiceTemplateId, resourceRp);

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
        [Route("{organizationId:guid}/servicetemplates/{projectServiceTemplateId:guid}")]
        public async Task<IActionResult> DeleteCloudProviderService(Guid organizationId, Guid projectServiceTemplateId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationProjectServiceTemplateService.DeleteOrganizationProjectServiceTemplate(organizationId, projectServiceTemplateId);

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
    }
}
