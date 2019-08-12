using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;

namespace PipelineSpace.Worker.Monitor
{
    public static class TelemetryClientManager
    {
        private static TelemetryClient _instance;
        public static TelemetryClient Instance
        {
            get
            {
                if(_instance == null)
                {
                    string key = TelemetryConfiguration.Active.InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY_SPACE", EnvironmentVariableTarget.Process) ?? "b8d28d8b-xxxx-xxxx-xxxx-7ceedc29b957";
                    _instance = new TelemetryClient() { InstrumentationKey = key };
                }

                return _instance;
            } 
        }

        public static IOperationHolder<RequestTelemetry> Operation;
    }
}
