using System;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Core
{
    public interface IEventHandler<T>
    {
        Task Handle(T @event);
    }
}
