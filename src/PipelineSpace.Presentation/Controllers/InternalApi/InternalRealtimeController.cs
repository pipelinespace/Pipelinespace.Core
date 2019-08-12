using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PipelineSpace.Domain.Core.Manager;
using PipelineSpace.Presentation.Constants;
using PipelineSpace.Presentation.Hubs;
using PipelineSpace.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.InternalApi
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "internaladmin")]
    [Route("internalapi/realtime/organizations")]
    public class InternalRealtimeController : BaseController
    {
        readonly IDomainManagerService _domainManagerService;
        readonly IHubContext<PipelineSpaceHub> _hubContext;
        readonly IHostingEnvironment _env;
        
        public InternalRealtimeController(IDomainManagerService domainManagerService,
            IHostingEnvironment env,
            IHubContext<PipelineSpaceHub> hubContext) : base(domainManagerService)
        {
            _domainManagerService = domainManagerService;
            this._hubContext = hubContext;
            this._env = env;
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/activities")]
        public async Task<IActionResult> NotifyProjectStatus(Guid organizationId, Guid projectId, [FromBody]RealtimePostRp model)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await this._hubContext.Clients.User(model.UserId).SendAsync(RealtimeConstants.ProjectUpdateStatus,
                Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    organizationId = organizationId,
                    projectId = projectId,
                    activityName = model.ActivityName,
                    status = model.Status
                }));

            return this.Ok();
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/services/{serviceId:guid}/activities")]
        public async Task<IActionResult> NotifyProjectServiceStatus(Guid organizationId, Guid projectId, Guid serviceId, [FromBody]RealtimePostRp model)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await this._hubContext.Clients.User(model.UserId).SendAsync(RealtimeConstants.ProjectServiceUpdateStatus,
                Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    organizationId = organizationId,
                    projectId = projectId,
                    serviceId = serviceId,
                    activityName = model.ActivityName,
                    status = model.Status
                }));

            return this.Ok();
        }

        [HttpPost]
        [Route("{organizationId:guid}/projects/{projectId:guid}/features/{featureId:guid}/services/{serviceId:guid}/activities")]
        public async Task<IActionResult> NotifyFeatureServiceStatus(Guid organizationId, Guid projectId, Guid featureId, Guid serviceId, [FromBody]RealtimePostRp model)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            await this._hubContext.Clients.User(model.UserId).SendAsync(RealtimeConstants.ProjectFeatureUpdateStatus,
                Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    organizationId = organizationId,
                    projectId = projectId,
                    featureId = featureId,
                    activityName = model.ActivityName,
                    status = model.Status
                }));

            return this.Ok();
        }

    }
}
