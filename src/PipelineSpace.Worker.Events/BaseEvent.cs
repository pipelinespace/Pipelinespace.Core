using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Worker.Events
{
    public class BaseEvent
    {
        public readonly string CorrelationId;
        public BaseEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
