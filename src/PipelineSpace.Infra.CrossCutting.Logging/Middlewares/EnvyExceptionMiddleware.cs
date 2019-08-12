using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PipelineSpace.Infra.CrossCutting.Logging.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.CrossCutting.Logging.Middlewares
{
    public sealed class EnvyExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionHandlerOptions _options;
        private readonly Func<object, Task> _clearCacheHeadersDelegate;
        private string _product, _layer;

        public EnvyExceptionHandlerMiddleware(string product, string layer,
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IOptions<ExceptionHandlerOptions> options,
            DiagnosticSource diagSource)
        {
            _product = product;
            _layer = layer;

            _next = next;
            _options = options.Value;
            _clearCacheHeadersDelegate = ClearCacheHeaders;
            if (_options.ExceptionHandler == null)
            {
                _options.ExceptionHandler = _next;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                bool isWebApi = context.Request.Path.StartsWithSegments("/api") ||
                                context.Request.Path.StartsWithSegments("/internalapi") ||
                                context.Request.Path.StartsWithSegments("/publicapi");

                if (isWebApi)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    EnvyWebHelper.LogWebError("Core", "Api", ex, context);

                    var errorId = Activity.Current?.Id ?? context.TraceIdentifier;
                    var jsonResponse = JsonConvert.SerializeObject(new EnvyErrorResponse
                    {
                        ErrorId = errorId,
                        Message = "Some kind of error happened in the API"
                    });
                    await context.Response.WriteAsync(jsonResponse, Encoding.UTF8);
                }
                else
                {
                    EnvyWebHelper.LogWebError(_product, _layer, ex, context);

                    PathString originalPath = context.Request.Path;
                    if (_options.ExceptionHandlingPath.HasValue)
                    {
                        context.Request.Path = _options.ExceptionHandlingPath;
                    }

                    context.Response.Clear();
                    var exceptionHandlerFeature = new ExceptionHandlerFeature()
                    {
                        Error = ex,
                        Path = originalPath.Value,
                    };

                    context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);
                    context.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);
                    context.Response.StatusCode = 500;
                    context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);

                    await _options.ExceptionHandler(context);

                    return;
                }
            }
        }

        private Task ClearCacheHeaders(object state)
        {
            var response = (HttpResponse)state;
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
            return Task.CompletedTask;
        }
    }
}
