using PipelineSpace.Application.Interfaces;
using PipelineSpace.Worker.Handlers.Core;
using System;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Messaging.InMemory
{
    public class InMemoryEventBusService : IEventBusService
    {
        private readonly IServiceProvider _serviceProvider;
        public InMemoryEventBusService(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task Publish<T>(string queueName, T @event)
        {
            var handler = (IEventHandler<T>)this._serviceProvider.GetService(typeof(IEventHandler<T>));
            await handler.Handle(@event);
        }
    }
}
