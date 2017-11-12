using System;
using System.Collections.Generic;
using AutoMapper;
using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Subscriber.Audit;
using EHR.ServerEvent.Subscriber.Audit.Extensions;
using EHR.ServerEvent.Subscriber.Audit.Mapping;
using EHR.ServerEvent.Subscriber.Cds;
using EHR.ServerEvent.Subscriber.CDS.Extensions;
using EHR.ServerEvent.Subscriber.CDS.Mapping;
using EHR.ServerEvent.Subscriber.Config;
using EHR.ServerEvent.Subscriber.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EHR.ServerEvent.Subscriber
{
    public class Startup
    {
        private const string WriteToCdsPg = "cds-pg";
        private const string WriteToAuditElastic = "elasticsearch";

        public IConfigurationRoot Configuration { get; }
        private readonly IDictionary<string, Type> _writeToMap = new Dictionary<string, Type>
        {
            {WriteToAuditElastic, typeof(AuditElasticWriter)},
            {"file", typeof(AuditFilleWriter)},
            {WriteToCdsPg, typeof(CdsDbWriter)},
        };

        private readonly IDictionary<string, Type> _transformToMap = new Dictionary<string, Type>
        {
            {"audit", typeof(AuditServerEventTransformer)},
            {"cds", typeof(CdsServerEventTransformer)},
        };

        private readonly IDictionary<string, Type> _transformToProcessorMap = new Dictionary<string, Type>
        {
            {"audit", typeof(RabbitMessageProcessor<ServerEventMessage,AuditEventMessage>)},
            {"cds", typeof(RabbitMessageProcessor<ServerEventMessage,CdsEventMessage>)}
        };


        public Startup()
            : this(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables())
        {
        }

        public Startup(IConfigurationBuilder builder)
        {
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //config
            services.AddLogging();
            services.AddSingleton(Configuration);
            var appsettings = new AppSettings();
            var rabbitConfig = new RabbitConfig();
            Configuration.Bind(appsettings);
            Configuration.GetSection("RabbitConfig").Bind(rabbitConfig);
            
            //config
            services.AddOptions();
            services.Configure<AppSettings>(Configuration);
            services.Configure<RabbitConfig>(Configuration.GetSection("RabbitConfig"));
            
            //message processing services
            RegisterWriter(services, appsettings.WriteTo);
            RegisterTransform(services, appsettings.TransformTo);
            RegisterProcessor(services, appsettings.TransformTo);

            //mapping
            services.AddAutoMapper(c =>
            {
                c.AddProfile<AuditEventMappingProfile>();
                c.AddProfile<CdsEventMappingProfile>();
            });

           
            //RabbitMQ
            services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(new RabbitMQ.Client.ConnectionFactory
            {
                HostName = rabbitConfig.HostName,
                Port = rabbitConfig.Port,
                UserName = rabbitConfig.Username,
                Password = rabbitConfig.Password,
                VirtualHost = rabbitConfig.VirtualHost
            });
            services.AddSingleton(_ => _.GetService<RabbitMQ.Client.IConnectionFactory>()
                    .CreateConnection($"EHR.ServerEvent.Subscriber-{appsettings.TransformTo}-{appsettings.WriteTo}"));
            services.AddTransient(_ => _.GetService<RabbitMQ.Client.IConnection>().CreateModel());

        }
        
        private void RegisterTransform(IServiceCollection services, string transformTo)
        {
            var t = GetTypeFromMap(_transformToMap, transformTo);
            services.AddTypeWithInterfaces(t);
        }
        
        private void RegisterProcessor(IServiceCollection services, string transformTo)
        {
            var t = GetTypeFromMap(_transformToProcessorMap, transformTo);
            services.AddTypeWithInterfaces(t);
        }
        
        private void RegisterWriter(IServiceCollection services, string writeTo)
        {
            var t = GetTypeFromMap(_writeToMap, writeTo);
            services.AddTypeWithInterfaces(t);

            if (writeTo.Equals(WriteToCdsPg))
            {
                services.UseCds(Configuration);
            }
            if (writeTo.Equals(WriteToAuditElastic))
            {
                services.UseAuditElastic(Configuration);
            }
        }

        #region util
        
        private Type GetTypeFromMap(IDictionary<string, Type> map, string key)
        {
            if (!map.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key),
                    $"{nameof(key)} should be one of: {string.Join(", ", map.Keys)}");
            }
            var t = map[key];
            return t;
        }

        #endregion
    }
}
