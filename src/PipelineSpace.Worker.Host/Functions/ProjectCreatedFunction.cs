using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using PipelineSpace.Worker.Host.DI;
using PipelineSpace.Worker.Handlers.Core;
using PipelineSpace.Worker.Events;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PipelineSpace.Worker.Host.Functions
{
    public static class ProjectCreatedFunction
    {
        [FunctionName("ProjectCreatedEvent")]
        public static async Task Run([ServiceBusTrigger("ProjectCreatedEvent", Connection = "ServiceBusConnection")]
                               string queueMessage,
                               ILogger log,
                               [Inject]IEventHandler<ProjectCreatedEvent> handler)
        {
            await handler.Handle(JsonConvert.DeserializeObject<ProjectCreatedEvent>(queueMessage));
        }
    }
}
