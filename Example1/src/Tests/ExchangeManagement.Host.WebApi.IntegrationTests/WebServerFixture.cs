using System;
using System.Net.Http;
using System.Web.Http;
using Autofac;
using CommonServiceLocator.AutofacAdapter.Unofficial;
using Microsoft.Owin.Testing;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using RestSharpClient;
using RestSharpClient.Contracts;

namespace ExchangeManagement.Host.WebApi.IntegrationTests
{
    public class TestStarup : Startup
    {
        protected override void RegisterDependency(ContainerBuilder builder)
        {
            builder.RegisterType<DapperUnitOfWork>().AsImplementedInterfaces().SingleInstance().OnActivated(e => e.Instance.PrventCommit());
            
            base.RegisterDependency(builder);
        }

        protected override void ResolveComponents(ILifetimeScope container)
        {
            var csl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => csl);
        }

        protected override void ConfigureDocumentations(HttpConfiguration httpConfig)
        {
            //В тестах документация не нужна
        }
    }

    public class WebServerFixture : IDisposable
    {
        private TestServer _webServer;
        public WebServerFixture()
        {
            _webServer = TestServer.Create<TestStarup>();
            UnitOfWork = ServiceLocator.Current.GetInstance<IUnitOfWork>();
        }

        public HttpClient AuthorizeClient
        {
            get
            {
                //todo RequestToken
                var client = _webServer.HttpClient;
                return client;
            }
        }

        public IRestClient RestClientNotAuthorize
        {
            get
            {
                return new HttpMessageHandlerClient(() => _webServer.Handler,
                    "http://localhost",
                    null,
                    new DelegatedDeserializer(JsonConvert.DeserializeObject),
                    new DelegatedSerializer(JsonConvert.SerializeObject),
                    TimeSpan.FromSeconds(10),
                    string.Empty);
            }
        }


        public HttpClient ClientNotAuthorize
        {
            get
            {
                var client = _webServer.HttpClient;
                client.DefaultRequestHeaders.Authorization = null;
                return client;
            }
        }

        public IUnitOfWork UnitOfWork { get; private set; }

        public void Dispose()
        {
            if (_webServer == null)
            {
                return;
            }

            _webServer.Dispose();
            _webServer = null;
        }
    }
}
