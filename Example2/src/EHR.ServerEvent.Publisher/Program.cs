using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Publisher.Config;
using EHR.ServerEvent.Publisher.Dequeue.Postgres;
using EHR.ServerEvent.Publisher.Mappers;
using EHR.ServerEvent.Publisher.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProtoBuf;

namespace EHR.ServerEvent.Publisher
{
    class Program
    {
        private static ILogger logger;
        private static readonly IList<IMessageReceiver> receivers = new List<IMessageReceiver>();

        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(services);
            var sp = services.BuildServiceProvider();
            var loggerFactory = sp.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(startup.Configuration.GetSection("Logging"));

            logger = sp.GetService<ILogger<Program>>();
            
            var proto = Serializer.GetProto<ServerEventMessage>();
            logger.LogInformation(".proto is:"+Environment.NewLine+proto);

            Console.CancelKeyPress += (s, e) =>
            {
                Stop();
                loggerFactory.Dispose();
            };

          
            var poolConfig = startup.Configuration.GetSection("Pooling").Get<PoolingConfig>();

            Start(poolConfig.ReceiversCount,
                () => sp.GetService<IMessageReceiver>(),
                () => sp.GetService<IPublisher>(), () => sp.GetService<Mapper>());

          

        }

        private static void Stop()
        {
            foreach (var receiver in receivers)
            {
                receiver.Stop();
            }
        }

        private static void Start(int receiversCount, 
            Func<IMessageReceiver> receiverFactory, 
            Func<IPublisher> publisherFactory, Func<Mapper> mapperFactory)
        {
            for (var i = 0; i < receiversCount; i++)
            {
                var receiver = receiverFactory.Invoke();
                var publisher = publisherFactory.Invoke();
                var mapper = mapperFactory.Invoke();
                receiver.MessageReceived += (sender, eventArgs) =>
                {
                    try
                    {
                       var serverEvents= mapper.ConvertToServerEvent(eventArgs.Message.ActionMetadata).ToList();
                        foreach (var serverEvent in serverEvents)
                        {
                            (Task.Run(async () => { await publisher.SendServerEventAsync(serverEvent); })).Wait();
                          
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Erro publish message. Exception.message {0}. Exception {1}", ex.Message, ex);
                        throw;
                    }
                };
                receiver.Start();
                receivers.Add(receiver);
            }
        }
    }
}