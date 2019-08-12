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
    public class OrganizationCPSController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IOrganizationCPSService _organizationCPSService;
        readonly IOrganizationCPSQueryService _organizationCPSQueryService;

        public OrganizationCPSController(IDomainManagerService domainManagerService,
                                         IOrganizationCPSService organizationCPSService,
                                         IOrganizationCPSQueryService organizationCPSQueryService) : base (domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _organizationCPSService = organizationCPSService;
            _organizationCPSQueryService = organizationCPSQueryService;
        }

        [HttpGet]
        [Route("{organizationId:guid}/cloudproviderservices")]
        public async Task<IActionResult> GetOrganizationCloudProviderServices(Guid organizationId)
        {
            var cloudProviderServices = await _organizationCPSQueryService.GetOrganizationCloudProviderServices(organizationId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            return this.Ok(cloudProviderServices);
        }

        [HttpGet]
        [Route("{organizationId:guid}/cloudproviderservices/{organizationCPSId:guid}")]
        public async Task<IActionResult> GetOrganizationCloudProviderServiceById(Guid organizationId, Guid organizationCPSId)
        {
            var cloudProviderService = await _organizationCPSQueryService.GetOrganizationCloudProviderServiceById(organizationId, organizationCPSId);

            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (cloudProviderService == null)
                return this.NotFound();

            return this.Ok(cloudProviderService);
        }

        [HttpPost]
        [Route("{organizationId:guid}/cloudproviderservices")]
        public async Task<IActionResult> CreateCloudProviderService(Guid organizationId, [FromBody]OrganizationCPSPostRp organizationCPSRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            if (organizationCPSRp.Type == Domain.Models.CloudProviderService.AWS && string.IsNullOrEmpty(organizationCPSRp.AccessSecret))
            {
                ModelState.AddModelError("", "Access Secret is required");
                return this.BadRequest(ModelState);
            }

            if (organizationCPSRp.Type == Domain.Models.CloudProviderService.Azure)
            {
                if (string.IsNullOrEmpty(organizationCPSRp.AccessName))
                {
                    ModelState.AddModelError("", "Subscription Name is required");
                }

                if (string.IsNullOrEmpty(organizationCPSRp.AccessAppId))
                {
                    ModelState.AddModelError("", "Application Id is required");
                }

                if (string.IsNullOrEmpty(organizationCPSRp.AccessAppSecret))
                {
                    ModelState.AddModelError("", "Application Secret is required");
                }

                if (string.IsNullOrEmpty(organizationCPSRp.AccessDirectory))
                {
                    ModelState.AddModelError("", "Directory is required");
                }

                if(ModelState.ErrorCount > 0)
                {
                    return this.BadRequest(ModelState);
                }
            }

            await _organizationCPSService.CreateCloudProviderService(organizationId, organizationCPSRp);

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
        [Route("{organizationId:guid}/cloudproviderservices/{organizationCPSId:guid}")]
        public async Task<IActionResult> UpdateCloudProviderService(Guid organizationId, Guid organizationCPSId, [FromBody]OrganizationCPSPutRp organizationCPSRp)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            if (organizationCPSRp.Type == Domain.Models.CloudProviderService.AWS && string.IsNullOrEmpty(organizationCPSRp.AccessSecret))
            {
                ModelState.AddModelError("", "Access Secret is required");
                return this.BadRequest(ModelState);
            }

            if (organizationCPSRp.Type == Domain.Models.CloudProviderService.Azure)
            {
                if (string.IsNullOrEmpty(organizationCPSRp.AccessName))
                {
                    ModelState.AddModelError("", "Subscription Name is required");
                }

                if (string.IsNullOrEmpty(organizationCPSRp.AccessAppId))
                {
                    ModelState.AddModelError("", "Application Id is required");
                }

                if (string.IsNullOrEmpty(organizationCPSRp.AccessAppSecret))
                {
                    ModelState.AddModelError("", "Application Secret is required");
                }

                if (string.IsNullOrEmpty(organizationCPSRp.AccessDirectory))
                {
                    ModelState.AddModelError("", "Directory is required");
                }

                if (ModelState.ErrorCount > 0)
                {
                    return this.BadRequest(ModelState);
                }
            }

            await _organizationCPSService.UpdateCloudProviderService(organizationId, organizationCPSId, organizationCPSRp);

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
        [Route("{organizationId:guid}/cloudproviderservices/{organizationCPSId:guid}")]
        public async Task<IActionResult> DeleteCloudProviderService(Guid organizationId, Guid organizationCPSId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await _organizationCPSService.DeleteCloudProviderService(organizationId, organizationCPSId);

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
