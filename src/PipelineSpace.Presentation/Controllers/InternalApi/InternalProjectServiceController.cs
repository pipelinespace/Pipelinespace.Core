using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Application.Services.InternalServices.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.InternalApi
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "internaladmin")]
    [Route("internalapi/organizations")]
    public class InternalProjectServiceController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IInternalProjectServiceService _internalProjectServiceService;
        readonly IProjectServiceService _projectServiceService;

        public InternalProjectServiceController(IDomainManagerService domainManagerService,
                                                IInternalProjectServiceService internalProjectServiceService,
                                                IProjectServiceService projectServiceService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _internalProjectServiceService = internalProjectServiceService;
            _projectServiceService = projectServiceService;
        }

        [HttpPatch]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}")]
        public async Task<IActionResult> PatchService(Guid organizationId, Guid projectId, Guid serviceId, [FromBody]ProjectServicePatchtRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectServiceService.PatchProjectService(organizationId, projectId, serviceId, resource);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }

        [HttpPatch]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/activate")]
        public async Task<IActionResult> ActivateService(Guid organizationId, Guid projectId, Guid serviceId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _internalProjectServiceService.ActivateProjectService(organizationId, projectId, serviceId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services")]
        public async Task<IActionResult> CreateProjectService(Guid organizationId, Guid projectId, [FromBody]InternalProjectServicePostRp projectServiceRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            ProjectServicePostRp resource = new ProjectServicePostRp();
            resource.Name = projectServiceRp.Name;
            resource.Description = projectServiceRp.Description;
            resource.ProjectServiceTemplateId = projectServiceRp.ProjectServiceTemplateId;

            await _projectServiceService.CreateProjectService(organizationId, projectId, resource, projectServiceRp.UserId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/imports")]
        public async Task<IActionResult> ImportProjectService(Guid organizationId, Guid projectId, [FromBody]InternalProjectServiceImportPostRp projectServiceRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            ProjectServiceImportPostRp resource = new ProjectServiceImportPostRp();
            resource.Name = projectServiceRp.Name;
            resource.Description = projectServiceRp.Description;
            resource.ServiceExternalId = projectServiceRp.ServiceExternalId;
            resource.ServiceExternalUrl = projectServiceRp.ServiceExternalUrl;
            resource.ServiceExternalName = projectServiceRp.ServiceExternalName;
            resource.BranchName = projectServiceRp.BranchName;
            resource.OrganizationCMSId = projectServiceRp.OrganizationCMSId;
            resource.BuildDefinitionYML = projectServiceRp.BuildDefinitionYML;
            resource.ProjectExternalId = projectServiceRp.ProjectExternalId;
            resource.ProjectExternalName = projectServiceRp.ProjectExternalName;

            resource.ProjectServiceTemplateId = projectServiceRp.ProjectServiceTemplateId;
            
            await _projectServiceService.ImportProjectService(organizationId, projectId, resource, projectServiceRp.UserId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok();
        }
    }
}
