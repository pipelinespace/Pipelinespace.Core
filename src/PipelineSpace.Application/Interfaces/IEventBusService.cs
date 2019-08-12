using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Application.Interfaces
{
    public interface IEventBusService
    {
        Task Publish<T>(string queueName, T @event);
    }
}
