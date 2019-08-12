using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.CrossCutting.Logging.Filters
{
    public class EnvyTrackPerformanceFilter : IActionFilter
    {
        private EnvyPerformanceTracker _tracker;
        private string _product, _layer;
        public EnvyTrackPerformanceFilter(string product, string layer)
        {
            _product = product;
            _layer = layer;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Request.Path.StartsWithSegments("/api") ||
               context.HttpContext.Request.Path.StartsWithSegments("/internalapi") ||
               context.HttpContext.Request.Path.StartsWithSegments("/publicapi"))
            {
                _layer = "Api";
            }
            else
            {
                _layer = "Mvc";
            }

            var request = context.HttpContext.Request;
            var activity = $"{request.Path}-{request.Method}";

            var dict = new Dictionary<string, object>();
            foreach (var key in context.RouteData.Values?.Keys)
                dict.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);

            var details = EnvyWebHelper.GetWebFlogDetail(_product, _layer, activity,
                context.HttpContext, dict);

            _tracker = new EnvyPerformanceTracker(details);
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (_tracker != null)
                _tracker.Stop();
        }
    }
}
