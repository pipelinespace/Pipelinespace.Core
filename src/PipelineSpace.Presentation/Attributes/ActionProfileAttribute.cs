using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Attributes
{
    public class ActionProfileAttribute : ActionMethodSelectorAttribute
    {
        public string Name { get; set; }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            if (!routeContext.HttpContext.Request.Query.Keys.Any(x => x.Equals("$profile", StringComparison.CurrentCultureIgnoreCase)))
            {
                return false;
            }

            var profile = routeContext.HttpContext.Request.Query["$profile"][0];
            if (string.IsNullOrEmpty(profile))
                return true;

            return profile.Equals(this.Name, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
