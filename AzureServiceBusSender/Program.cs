using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureServiceBusSender
{
    class Program
    {
        static IConfigurationRoot configurationRoot;
        static IServiceProvider serviceProvider;

        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            
            Sender sender = serviceProvider.GetService<Sender>();
            await sender.SendMessageToQueueAsync(new { CurrentTime = DateTime.UtcNow });
            await sender.SendMessageToTopicAsync(new { CurrentTime = DateTime.UtcNow });

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration.Sources.Clear();

                IHostEnvironment env = hostingContext.HostingEnvironment;
                configuration
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables(); 

                configurationRoot = configuration.Build();
            })
            .ConfigureServices((_, services) =>
            {
                ServiceBusConfig config = new();
                configurationRoot.GetSection(nameof(ServiceBusConfig)).Bind(config);
                services.AddSingleton(config);
                services.AddSingleton<Sender>();
                serviceProvider = services.BuildServiceProvider();
            });
    }
}
