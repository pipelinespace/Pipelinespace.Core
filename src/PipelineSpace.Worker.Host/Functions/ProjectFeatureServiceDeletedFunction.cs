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
    public static class ProjectFeatureServiceDeletedFunction
    {
        [FunctionName("ProjectFeatureServiceDeletedEvent")]
        public static async Task Run([ServiceBusTrigger("ProjectFeatureServiceDeletedEvent", Connection = "ServiceBusConnection")]
                               string queueMessage,
                               ILogger log,
                               [Inject]IEventHandler<ProjectFeatureServiceDeletedEvent> handler)
        {
            await handler.Handle(JsonConvert.DeserializeObject<ProjectFeatureServiceDeletedEvent>(queueMessage));
        }
    }
}
