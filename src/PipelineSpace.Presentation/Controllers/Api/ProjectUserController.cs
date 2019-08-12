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
    [Authorize(Roles = "globaladmin,organizationadmin,projectadmin")]
    [Route("{api}/organizations")]
    public class ProjectUserController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;

        readonly IProjectUserService _projectUserService;
        readonly IProjectUserQueryService _projectUserQueryService;

        public ProjectUserController(IDomainManagerService domainManagerService,
                                     IProjectUserService projectUserService,
                                     IProjectUserQueryService projectUserQueryService) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            _projectUserService = projectUserService;
            _projectUserQueryService = projectUserQueryService;
        }

        
        [HttpGet]
        [Route("{organizationId:guid}/projects/{projectId:guid}/users")]
        public async Task<IActionResult> GetUsers(Guid organizationId, Guid projectId)
        {
            var users = await _projectUserQueryService.GetUsers(organizationId, projectId);
            
            if (_domainManagerService.HasNotFounds())
            {
                return this.NotFound(_domainManagerService.GetNotFounds());
            }

            if (_domainManagerService.HasForbidden())
            {
                return this.Forbidden(_domainManagerService.GetForbidden());
            }

            return this.Ok(users);
        }

        [HttpDelete]
        [Route("{organizationId:guid}/projects/{projectId:guid}/users/{userId}")]
        public async Task<IActionResult> RemoveUser(Guid organizationId, Guid projectId, string userId)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);
            
            await _projectUserService.RemoveUser(organizationId, projectId, userId);

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
