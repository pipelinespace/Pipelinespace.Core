using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using PipelineSpace.Worker.Host.DI;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Events;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PipelineSpace.Worker.Host.Functions
{
    public static class ProjectServiceBuildQueuedFunction
    {
        [FunctionName("ProjectServiceBuildQueuedEvent")]
        public static async Task Run([ServiceBusTrigger("ProjectServiceBuildQueuedEvent", Connection = "ServiceBusConnection")]
                               string queueMessage,
                               ILogger log,
                               [Inject]IEventHandler<ProjectServiceBuildQueuedEvent> handler)
        {
            await handler.Handle(JsonConvert.DeserializeObject<ProjectServiceBuildQueuedEvent>(queueMessage));
        }
    }
}
