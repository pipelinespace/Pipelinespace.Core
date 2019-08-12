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
    [Route("{api}/stats")]
    public class StatController : BaseController
    {
        readonly IDashboardQueryService _dashboardQueryService;

        public StatController(
            IDomainManagerService domainManagerService,
            IDashboardQueryService dashboardQueryService) : base(domainManagerService)
        {
            _dashboardQueryService = dashboardQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var dashboard = await this._dashboardQueryService.GetStats();
            return this.Ok(dashboard);
        }
    }
}
