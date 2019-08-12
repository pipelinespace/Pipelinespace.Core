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
    public static class EnvyExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseEnvyExceptionHandler(
            this IApplicationBuilder builder, string errorHandlingPath)
        {
            return builder.UseMiddleware<EnvyExceptionHandlerMiddleware>
                    ("Core", "Mvc", Options.Create(new ExceptionHandlerOptions
                    {
                        ExceptionHandlingPath = new PathString(errorHandlingPath)
                    }));
        }
    }
}
