using System;
using System.Data;
using EHR.ServerEvent.Publisher.Config;
using EHR.ServerEvent.Publisher.Dequeue.Postgres;
using EHR.ServerEvent.Publisher.Mappers;
using EHR.ServerEvent.Publisher.Mappers.PayloadReader;
using EHR.ServerEvent.Publisher.Mappers.ResourceInformationReader;
using EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder;
using EHR.ServerEvent.Publisher.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Scrutor;

namespace EHR.ServerEvent.Publisher
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //config
            services.AddLogging();
            services.AddOptions();
            
            services.AddSingleton(Configuration);
            var poolingConfig = new PoolingConfig();
            
            Configuration.GetSection("Pooling").Bind(poolingConfig);
            services.AddSingleton(poolingConfig);
            services.AddSingleton(poolingConfig.PoolingOptions);

            services.Configure<RabbitMqConfig>(Configuration.GetSection("Rabbit"));

            //services
            services.AddTransient<IPublisher, RabbitPublisher>();
            services.AddSingleton<Func<IDbConnection>>(() => new NpgsqlConnection(Configuration.GetConnectionString("Fhirbase")));
            services.AddTransient<IMessageReceiver, MessageReceiver>();

            services.AddTransient<Mapper, Mapper>();
            services.AddSingleton<ResourceBuilderFuctory, ResourceBuilderFuctory>();
            services.Scan(scan => scan
                .FromAssemblyOf<Startup>()
                .AddClasses(filter => filter.AssignableTo(typeof(IResourceInformationBuilder)))
                .AsImplementedInterfaces().WithTransientLifetime());
            services.Scan(scan => scan
                .FromAssemblyOf<Startup>()
                .AddClasses(filter => filter.AssignableTo(typeof(ResourceInformationReader)))
                .As<ResourceInformationReader>().WithTransientLifetime());
            services.Scan(scan => scan
                .FromAssemblyOf<Startup>()
                .AddClasses(filter => filter.AssignableTo(typeof(PayloadReader)))
                .As<PayloadReader>().WithTransientLifetime());



            //RabbitMQ
            services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(sp => 
            {
                var rabbitConfig = sp.GetService<IOptions<RabbitMqConfig>>().Value;
                return new RabbitMQ.Client.ConnectionFactory{
                    HostName = rabbitConfig.HostName,
                    Port = rabbitConfig.Port,
                    UserName = rabbitConfig.Username,
                    Password = rabbitConfig.Password,
                    VirtualHost = rabbitConfig.VirtualHost
                };
        });
            
            services.AddSingleton(_ => _.GetService<RabbitMQ.Client.IConnectionFactory>().CreateConnection("EHR.ServerEvent.Publisher"));
            services.AddTransient(_ => _.GetService<RabbitMQ.Client.IConnection>().CreateModel());

        }
    }
}
