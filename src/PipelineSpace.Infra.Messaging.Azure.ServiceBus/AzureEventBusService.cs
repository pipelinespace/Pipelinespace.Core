using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using PipelineSpace.Application.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Messaging.Azure.ServiceBus
{
    public class AzureEventBusService : IEventBusService
    {
        readonly IConfiguration _configuration;
        public AzureEventBusService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Publish<T>(string queueName, T @event)
        {
            QueueClient queueClient = new QueueClient(_configuration["ConnectionStrings:ServiceBusConnection"], queueName);
            var message = new Message(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(@event)));
            await queueClient.SendAsync(message);
        }
    }
}
