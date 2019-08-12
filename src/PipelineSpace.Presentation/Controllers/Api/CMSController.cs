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
    [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
    [Route("{api}/cms")]
    public class CMSController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly ICMSExternalQueryService _cmsExternalQueryService;

        public CMSController(IDomainManagerService domainManagerService,
                                 ICMSExternalQueryService cmsExternalQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _cmsExternalQueryService = cmsExternalQueryService;
        }

        [HttpGet]
        [Route("{cmsProvider}/accounts")]
        public async Task<IActionResult> GetAccounts(string cmsProvider)
        {
            ConfigurationManagementService cmsValue = (ConfigurationManagementService)Enum.Parse(typeof(ConfigurationManagementService), cmsProvider);

            var accounts = await _cmsExternalQueryService.GetAccounts(cmsValue);
            return this.Ok(accounts);
        }
        
    }
}
