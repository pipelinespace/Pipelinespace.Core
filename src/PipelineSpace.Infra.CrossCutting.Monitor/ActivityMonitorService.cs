using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PipelineSpace.Application.Interfaces;
using System;

namespace PipelineSpace.Infra.CrossCutting.Monitor
{
    public class ActivityMonitorService : IActivityMonitorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActivityMonitorService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public string GetCorrelationId()
        {
            RequestTelemetry requestTelemetry = _httpContextAccessor.HttpContext.Features.Get<RequestTelemetry>();
            return requestTelemetry.Id ?? string.Empty;
        }
    }
}
