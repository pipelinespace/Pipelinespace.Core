using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin")]
    [Route("{api}/servicetemplates")]
    public class ProjectServiceTemplateController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IProjectServiceTemplateQueryService _projectServiceTemplateQueryService;

        public ProjectServiceTemplateController(IDomainManagerService domainManagerService,
                                                IProjectServiceTemplateQueryService projectServiceTemplateQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectServiceTemplateQueryService = projectServiceTemplateQueryService;
        }

        [HttpGet]
        [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
        public async Task<IActionResult> GetProjectServiceTemplates([FromQuery(Name = "gitProviderType")]ConfigurationManagementService gitProviderType,
                                                                    [FromQuery(Name = "cloudProviderType")]CloudProviderService cloudProviderType,
                                                                    [FromQuery(Name = "pipeType")]PipeType pipeType)
        {
            var serviceTemplates = await _projectServiceTemplateQueryService.GetProjectServiceTemplates(gitProviderType, cloudProviderType, pipeType);
            return this.Ok(serviceTemplates);
        }

        [HttpGet]
        [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
        [Route("{projectServiceTemplateId:guid}/definitions")]
        public async Task<IActionResult> GetProjectServiceTemplates(Guid projectServiceTemplateId)
        {
            var serviceTemplateDefinition = await _projectServiceTemplateQueryService.GetProjectServiceTemplateDefinition(projectServiceTemplateId);
            return this.Ok(serviceTemplateDefinition);
        }


        [HttpGet]
        [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
        [Route("internals")]
        public async Task<IActionResult> GetProjectServiceTemplateInternals([FromQuery(Name = "programmingLanguageId")]Guid programmingLanguageId,
                                                                            [FromQuery(Name = "cloudProviderType")]CloudProviderService cloudProviderType)
        {
            var serviceTemplates = await _projectServiceTemplateQueryService.GetProjectServiceTemplateInternals(programmingLanguageId, cloudProviderType);
            return this.Ok(serviceTemplates);
        }
    }
}
