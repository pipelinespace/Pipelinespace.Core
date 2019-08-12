using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Interfaces
{
    public interface IActivityMonitorService
    {
        string GetCorrelationId();
    }
}
