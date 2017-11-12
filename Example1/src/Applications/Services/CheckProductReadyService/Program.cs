using System;
using System.Configuration;
using System.Net.Http;
using Autofac;
using EasyNetQ;
using EasyNetQ.Serilog;
using ExchangeManagement.Contract.Messages;
using Newtonsoft.Json;
using RestSharpClient;
using RestSharpClient.Contracts;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using Serilog.Sinks.EventLog;
using Thinktecture.IdentityModel.Client;
using Topshelf;

namespace CheckProductReadyService
{
    class Program
    {
        static void Main()
        {
            var settings = new Settings();
            
            ILifetimeScope container = null;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .CreateLogger();
            
            var builder = new ContainerBuilder();

            var easyNetQLogger = new SerilogLogger(Log.Logger);
            
            builder.Register(t => settings).AsSelf();

            builder.Register(c => RabbitHutch.CreateBus(register => register.Register<IEasyNetQLogger>(l=>easyNetQLogger)).Advanced).As<IAdvancedBus>();
            
            builder.Register(t =>
            {
                var client = new OAuth2Client(
                    new Uri(settings.TokenEndpoint),
                    settings.Client,
                    settings.Clientsecret);

                var tokenResult = client.RequestClientCredentialsAsync(settings.RequestedScopes).Result;
                return tokenResult;
            });

            builder.RegisterType<Authenticator>().AsImplementedInterfaces();

            builder.Register(c =>
                new HttpMessageHandlerClient(
                    () => new HttpClientHandler(),
                    settings.MetainfoServiceEndpoint,
                    container.Resolve<IAuthenticator>(),
                    new DelegatedDeserializer(JsonConvert.DeserializeObject),
                    new DelegatedSerializer(JsonConvert.SerializeObject),
                    string.Empty)
            ).As<IRestClient>();
            
            builder.RegisterType<StubFinishedProductService>().AsImplementedInterfaces();

            builder.RegisterType<CheckProductReadyService>().AsSelf();

            container = builder.Build();

            HostFactory.Run(c =>
            {
                c.Service<CheckProductReadyService>((s) =>
                {
                    s.ConstructUsing(name => container.Resolve<CheckProductReadyService>());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });
                c.RunAsLocalSystem();

                c.SetServiceName(settings.ServiceName);
                c.SetDisplayName(settings.ServiceName);
                c.SetDescription("Сервис проверки готовности продукта");

                c.DependsOnEventLog();

                c.StartAutomatically();

                c.UseSerilog();

                c.EnableServiceRecovery(configurator =>
                {
                    configurator.RestartService(1);
                });
            });
        }
    }
}
