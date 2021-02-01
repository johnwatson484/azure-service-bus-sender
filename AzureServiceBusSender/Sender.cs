using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace AzureServiceBusSender
{
    class Sender
    {
        ServiceBusConfig config;
        string connectionString;

        public Sender(ServiceBusConfig config)
        {
            this.config = config;
            connectionString = CreateConnectionString(config);
        }

        public async Task SendMessageToQueueAsync(object body)
        {
            await using ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(config.Queue);
            ServiceBusMessage message = new ServiceBusMessage(SerializeMessage(body));
            await sender.SendMessageAsync(message);
            
            Console.WriteLine($"Sent message - {message.Body}");
        }

        public async Task SendMessageToTopicAsync(object body)
        {
            await using ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(config.Topic);
            ServiceBusMessage message = new ServiceBusMessage(SerializeMessage(body));
            await sender.SendMessageAsync(message);
        }

        private string CreateConnectionString(ServiceBusConfig config)
        {
            return $"Endpoint=sb://{config.Host}/;SharedAccessKeyName={config.Username};SharedAccessKey={config.Password}";
        }

        private string SerializeMessage(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
