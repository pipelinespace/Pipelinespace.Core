using Microsoft.Extensions.Configuration;
using PipelineSpace.Infra.CrossCutting.Logging.Models;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PipelineSpace.Infra.CrossCutting.Logging
{
    public static class EnvyLogger
    {
        private static ILogger _performanceLogger;
        private static ILogger _usageLogger;
        private static ILogger _errorLogger;
        private static ILogger _diagnosticLogger;

        public static void Configure(IConfiguration configuration)
        {
            _performanceLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration.GetSection("Logging:Performance"))
                .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                .ReadFrom.ConfigurationSection(configuration.GetSection("Logging:Usage"))
                .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                .ReadFrom.ConfigurationSection(configuration.GetSection("Logging:Error"))
                .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                .ReadFrom.ConfigurationSection(configuration.GetSection("Logging:Diagnostic"))
                .CreateLogger();
        }

        public static void WritePerf(EnvyLogDetail infoToLog)
        {
            _performanceLogger.Write(LogEventLevel.Information, "{@PipelineSpaceLogDetail}", infoToLog);
        }

        public static void WriteUsage(EnvyLogDetail infoToLog)
        {
            _usageLogger.Write(LogEventLevel.Information, "{@PipelineSpaceLogDetail}", infoToLog);
        }

        public static void WriteError(EnvyLogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                var procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName)
                    ? infoToLog.Location
                    : procName;
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }
            _errorLogger.Write(LogEventLevel.Information, "{@PipelineSpaceLogDetail}", infoToLog);
        }
        public static void WriteDiagnostic(EnvyLogDetail infoToLog)
        {
            var writeDiagnostics = Convert.ToBoolean(Environment.GetEnvironmentVariable("DIAGNOSTICS_ON"));
            if (!writeDiagnostics)
                return;

            _diagnosticLogger.Write(LogEventLevel.Information, "{@PipelineSpaceLogDetail}", infoToLog);
        }

        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }

        private static string FindProcName(Exception ex)
        {
            var sqlEx = ex as SqlException;
            if (sqlEx != null)
            {
                var procName = sqlEx.Procedure;
                if (!string.IsNullOrEmpty(procName))
                    return procName;
            }

            if (!string.IsNullOrEmpty((string)ex.Data["Procedure"]))
            {
                return (string)ex.Data["Procedure"];
            }

            if (ex.InnerException != null)
                return FindProcName(ex.InnerException);

            return null;
        }
    }
}
