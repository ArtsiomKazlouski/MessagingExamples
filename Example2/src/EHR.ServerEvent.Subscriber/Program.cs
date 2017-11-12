using System;
using System.IO;
using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Subscriber.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EHR.ServerEvent.Subscriber
{
    public class Program
    {
        private static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(services);
            var sp = services.BuildServiceProvider();
            
            var loggerFactory = sp.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(LogLevel.Debug);


            var processor = sp.GetService<IMessageProcessor<ServerEventMessage>>();
            
            Console.CancelKeyPress += (s, e) =>
            {
                try
                {
                    processor.Stop();
                }
                finally
                {
                    loggerFactory.Dispose();
                    processor.Dispose();
                }
            };
            
            processor.Start();
        }
    }
}