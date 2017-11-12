using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Autofac;
using EasyNetQ;
using EasyNetQ.Serilog;
using ExchangeManagement.Wrapper;
using Newtonsoft.Json;
using RestSharpClient;
using RestSharpClient.Contracts;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using Serilog.Sinks.EventLog;
using Thinktecture.IdentityModel.Client;
using Topshelf;

namespace NotifyRecipientsOfFinishedProductService
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

            builder.Register(c => RabbitHutch.CreateBus(register => register.Register<IEasyNetQLogger>(l => easyNetQLogger)).Advanced).As<IAdvancedBus>();

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
                    settings.SubscriptionServiceEndpoint, 
                    container.Resolve<IAuthenticator>(),
                    new DelegatedDeserializer(JsonConvert.DeserializeObject),
                    new DelegatedSerializer(JsonConvert.SerializeObject),
                    string.Empty)
            ).As<IRestClient>();

            builder.Register(c =>
                    {
                        var client = new HttpClient
                        {
                            BaseAddress = new Uri(settings.MetainfoServiceEndpoint)
                        };
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        return client;
                    })
                .As<HttpClient>();
            
            builder.RegisterType<CheckSubscriptionService>().AsImplementedInterfaces();

            builder.RegisterType<SubscriptionService>().AsImplementedInterfaces();

            builder.RegisterType<ImportService>().AsImplementedInterfaces();

            builder.RegisterType<Service>().AsSelf();

            container = builder.Build();

            HostFactory.Run(c =>
            {
                c.Service<Service>((s) =>
                {
                    s.ConstructUsing(name => container.Resolve<Service>());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });
                c.RunAsLocalSystem();

                c.SetServiceName(settings.ServiceName);
                c.SetDisplayName(settings.ServiceName);
                c.SetDescription("Сервис отправки готовой продукции");

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
