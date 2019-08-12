using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using PipelineSpace.Worker.Host.DI.Bindings;
using PipelineSpace.Worker.Monitor;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Host.DI.Services
{
    internal class ScopeCleanupFilter : IFunctionInvocationFilter, IFunctionExceptionFilter
    {
        private readonly ServiceProviderHolder _scopeHolder;

        public ScopeCleanupFilter(ServiceProviderHolder scopeHolder) =>
            _scopeHolder = scopeHolder;

        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            var baseEvent = JsonConvert.DeserializeObject<dynamic>(executingContext.Arguments.First().Value as string);
            string correlationId = baseEvent.CorrelationId;

            TelemetryClientManager.Instance.Context.Cloud.RoleName = executingContext.FunctionName;

            RequestTelemetry requestTelemetry = new RequestTelemetry { Name = executingContext.FunctionName };
            requestTelemetry.Context.Operation.Id = CorrelationHelper.GetOperationId(correlationId);
            requestTelemetry.Context.Operation.ParentId = correlationId;

            TelemetryClientManager.Operation = TelemetryClientManager.Instance.StartOperation(requestTelemetry);

            return Task.CompletedTask;
        }

        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            TelemetryClientManager.Operation.Telemetry.Success = true;
            TelemetryClientManager.Instance.StopOperation(TelemetryClientManager.Operation);

            _scopeHolder.RemoveScope(executedContext.FunctionInstanceId);
            return Task.CompletedTask;
        }

        public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
        {
            TelemetryClientManager.Instance.TrackException(exceptionContext.Exception);

            TelemetryClientManager.Operation.Telemetry.Success = false;
            TelemetryClientManager.Operation.Telemetry.ResponseCode = "500";

            TelemetryClientManager.Instance.StopOperation(TelemetryClientManager.Operation);

            _scopeHolder.RemoveScope(exceptionContext.FunctionInstanceId);
            return Task.CompletedTask;
        }
    }

    public static class CorrelationHelper
    {
        public static string GetOperationId(string requestId)
        {
            // Returns the root ID from the '|' to the first '.' if any.
            // Following the HTTP Protocol for Correlation - Hierarchical Request-Id schema is used
            // https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/HierarchicalRequestId.md
            int rootEnd = requestId.IndexOf('.');
            if (rootEnd < 0)
                rootEnd = requestId.Length;

            int rootStart = requestId[0] == '|' ? 1 : 0;
            return requestId.Substring(rootStart, rootEnd - rootStart);
        }
    }
}
