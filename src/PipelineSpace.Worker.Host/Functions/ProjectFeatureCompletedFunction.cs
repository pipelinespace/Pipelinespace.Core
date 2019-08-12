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
    public static class ProjectFeatureCompletedFunction
    {
        [FunctionName("ProjectFeatureCompletedEvent")]
        public static async Task Run([ServiceBusTrigger("ProjectFeatureCompletedEvent", Connection = "ServiceBusConnection")]
                               string queueMessage,
                               ILogger log,
                               [Inject]IEventHandler<ProjectFeatureCompletedEvent> handler)
        {
            await handler.Handle(JsonConvert.DeserializeObject<ProjectFeatureCompletedEvent>(queueMessage));
        }
    }
}
