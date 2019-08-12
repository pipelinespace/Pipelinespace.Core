using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PipelineSpace.Infra.CrossCutting.Logging.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PipelineSpace.Infra.CrossCutting.Logging.Middlewares
{
    public static class SpaceTelemetryMiddlewareExtensions
    {
        public static IApplicationBuilder UseSpaceLogging(
            this IApplicationBuilder builder, string errorHandlingPath)
        {
            return builder.UseMiddleware<SpaceTelemetryHandlerMiddleware>
                    (Options.Create(new ExceptionHandlerOptions
                    {
                        ExceptionHandlingPath = new PathString(errorHandlingPath)
                    }));
        }
    }
}
