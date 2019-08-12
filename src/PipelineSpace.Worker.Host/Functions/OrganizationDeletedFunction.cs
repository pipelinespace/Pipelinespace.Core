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
    public static class OrganizationDeletedFunction
    {
        [FunctionName("OrganizationDeletedEvent")]
        public static async Task Run([ServiceBusTrigger("OrganizationDeletedEvent", Connection = "ServiceBusConnection")]
                               string queueMessage,
                               ILogger log,
                               [Inject]IEventHandler<OrganizationDeletedEvent> handler)
        {
            await handler.Handle(JsonConvert.DeserializeObject<OrganizationDeletedEvent>(queueMessage));
        }
    }
}
