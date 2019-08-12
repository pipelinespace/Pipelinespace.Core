using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PipelineSpace.Infra.CrossCutting.Logging.Models;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.CrossCutting.Logging.Middlewares
{
    public sealed class SpaceTelemetryHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionHandlerOptions _options;
        static readonly ILogger Log = Serilog.Log.ForContext<SpaceTelemetryHandlerMiddleware>();
        const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        private readonly Func<object, Task> _clearCacheHeadersDelegate;
        private Stopwatch _sw;
        readonly IHostingEnvironment _hostingEnvironment;
        public SpaceTelemetryHandlerMiddleware(
            IOptions<ExceptionHandlerOptions> options,
            RequestDelegate next,
            DiagnosticSource diagSource,
            IHostingEnvironment hostingEnvironment)
        {
            _options = options.Value;
            _next = next;
            if (_options.ExceptionHandler == null)
            {
                _options.ExceptionHandler = _next;
            }
            _clearCacheHeadersDelegate = ClearCacheHeaders;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // _sw = Stopwatch.StartNew();

                await _next(context);

                // _sw.Stop();

                //LogForContext(context).Information(MessageTemplate, context.Request.Method, context.Request.Path, context.Response?.StatusCode, _sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                //_sw.Stop();

                var ai = new TelemetryClient();
                ai.TrackException(ex);

                // LogException(context, _sw.ElapsedMilliseconds, ex);

                bool isWebApi = context.Request.Path.StartsWithSegments("/api") ||
                                context.Request.Path.StartsWithSegments("/api-core") ||
                                context.Request.Path.StartsWithSegments("/internalapi") ||
                                context.Request.Path.StartsWithSegments("/publicapi");

                if (isWebApi)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorId = context.TraceIdentifier;
                    var jsonResponse = JsonConvert.SerializeObject(new EnvyErrorResponse
                    {
                        ErrorId = errorId,
                        Message = _hostingEnvironment.IsDevelopment() ? ex.ToString() : "Some kind of error happened in the API"
                    });
                    await context.Response.WriteAsync(jsonResponse, Encoding.UTF8);
                }
                else
                {
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

        static void LogException(HttpContext httpContext, double elapsedMs, Exception ex)
        {
            LogForContext(httpContext).Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, elapsedMs);
        }

        static ILogger LogForContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

            return result;
        }

        static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
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
