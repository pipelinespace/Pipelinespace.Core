using PipelineSpace.Infra.Options;
using PipelineSpace.Worker.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services.Interfaces
{
    public interface IProjectFeatureHandlerService
    {
        Task CompleteProjectFeature(ProjectFeatureCompletedEvent @event);
    }
}
