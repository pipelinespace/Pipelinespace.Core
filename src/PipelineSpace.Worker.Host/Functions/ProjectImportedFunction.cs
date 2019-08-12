using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using PipelineSpace.Worker.Host.DI;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Events;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PipelineSpace.Worker.Host.Functions
{
    public static class ProjectImportedFunction
    {
        [FunctionName("ProjectImportedEvent")]
        public static async Task Run([ServiceBusTrigger("ProjectImportedEvent", Connection = "ServiceBusConnection")]
                               string queueMessage,
                               ILogger log,
                               [Inject]IEventHandler<ProjectImportedEvent> handler)
        {
            await handler.Handle(JsonConvert.DeserializeObject<ProjectImportedEvent>(queueMessage));
        }
    }
}
