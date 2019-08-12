using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Application.Models;
using PipelineSpace.Application.Services.Interfaces;
using PipelineSpace.Domain.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Authorize(Roles = "globaladmin,organizationadmin")]
    [Route("{api}/organizations")]
    public class OrganizationController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IOrganizationService _organizationService;
        readonly IOrganizationQueryService _organizationQueryService;

        public OrganizationController(IDomainManagerService domainManagerService,
                                      IOrganizationService organizationService,
                                      IOrganizationQueryService organizationQueryService) : base (domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _organizationService = organizationService;
            _organizationQueryService = organizationQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganizations()
        {
            var organizations = await _organizationQueryService.GetOrganizations();
            return this.Ok(organizations);
        }

        [HttpGet]
        [Route("{organizationId:guid}")]
        public async Task<IActionResult> GetOrganizationById(Guid organizationId)
        {
            var organization = await _organizationQueryService.GetOrganizationById(organizationId);

            if (organization == null)
                return this.NotFound();

            return this.Ok(organization);
        }

        [HttpPost]
        [Authorize(Roles = "globaladmin,organizationadmin")]
        public async Task<IActionResult> CreateOrganization([FromBody]OrganizationPostRp organizationRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationService.CreateOrganization(organizationRp);

            if (_domainManagerService.HasConflicts())
            {
                return this.Conflict(_domainManagerService.GetConflicts());
            }

            return this.Ok(new { OrganizationId = await _domainManagerService.GetResult<Guid>("OrganizationId") });
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "globaladmin,organizationadmin")]
        public async Task<IActionResult> UpdateOrganization(Guid id, [FromBody]OrganizationPutRp organizationRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationService.UpdateOrganization(id, organizationRp);

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
        [Route("{id:guid}")]
        [Authorize(Roles = "globaladmin,organizationadmin")]
        public async Task<IActionResult> DeleteOrganization(Guid id)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationService.DeleteOrganization(id);

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
