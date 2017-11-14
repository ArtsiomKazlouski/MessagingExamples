using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Core;
using Autofac.Features.Variance;
using Autofac.Integration.WebApi;
using EasyNetQ;
using ExchangeManagement.Host.WebApi.Handlers;
using ExchangeManagement.Migrations.Installers;
using FluentMigrator.Runner.Initialization;
using FluentValidation;
using MediatR;
using Microsoft.Owin;
using Owin;
using SwaggerDocumentation;
using Module = Autofac.Module;

[assembly: OwinStartup(typeof(ExchangeManagement.Host.WebApi.Startup))]

namespace ExchangeManagement.Host.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<WebApiModule>()
                .RegisterModule<InfrastructureModule>();
            builder.RegisterAssemblyModules(typeof(MigrationsRunnerInstaller).Assembly);

            RegisterDependency(builder);
            ILifetimeScope container = builder.Build();

            ResolveComponents(container);

            HttpConfiguration httpConfiguration = container.Resolve<HttpConfiguration>();
            ConfigureDocumentations(httpConfiguration);
            app.UseAutofacMiddleware(container)
                .UseAutofacWebApi(httpConfiguration)
                .UseWebApi(httpConfiguration);

            //foreach (var context in container.Resolve<IEnumerable<RunnerContext>>())
            //{
            //    new CustomTaskExecutor(context).CheskForNotAppliedMigrations();
            //}
        }

        protected virtual void RegisterDependency(ContainerBuilder builder)
        {

        }

        protected virtual void ResolveComponents(ILifetimeScope container)
        {

        }

        protected virtual void ConfigureDocumentations(HttpConfiguration httpConfig)
        {

            var appSettings = ConfigurationManager.AppSettings;

            var documentationOauthConfig =
                new OauthSecuritySchema(
                    new Uri("localhost:8080"),
                    appSettings["swaggerOauth:ClientId"],
                    appSettings["swaggerOauth:RequestedScopes"].Split(' '));

            httpConfig.UseSwagger(Assembly.GetExecutingAssembly(), documentationOauthConfig);

        }
    }

    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //todo
            ////To deploy an application that uses spatial data types to a machine that does not have 'System CLR Types for SQL Server' installed you also need to deploy the native assembly SqlServerSpatial110.dll. Both x86 (32 bit) and x64 (64 bit) versions of this assembly have been added to your project under the SqlServerTypes\x86 and SqlServerTypes\x64 subdirectories. The native assembly msvcr100.dll is also included in case the C++ runtime is not installed.
            //// need to add code to load the correct one of these assemblies at runtime (depending on the current architecture).
            ////http://blogs.msdn.com/b/adonet/archive/2013/12/09/microsoft-sqlserver-types-nuget-package-spatial-on-azure.aspx
            //SqlServerTypes.Utilities.LoadNativeAssemblies(System.Web.HttpContext.Current != null
            //    ? System.Web.HttpContext.Current.Server.MapPath("~/bin")
            //    : (System.AppDomain.CurrentDomain.BaseDirectory));


            base.Load(builder);

            // builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
            
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(AbstractValidator<>)).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(ThisAssembly)
                .As(type => type.GetInterfaces()
                .Where(interfaceType => interfaceType.IsClosedTypeOf(typeof(IAsyncRequestHandler<,>)))
                .Select(it => new KeyedService("commandHandler", it))
                );

            builder.RegisterGenericDecorator(
               typeof(SqlExceptionHandler<,>),
               typeof(IAsyncRequestHandler<,>),
               fromKey: "commandHandler", toKey: "sqlExceptionHandler");
            
            builder.RegisterGenericDecorator(
              typeof(ValidatorHandler<,>),
              typeof(IAsyncRequestHandler<,>),
              fromKey: "sqlExceptionHandler");
            //MeditR registration
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                IComponentContext c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            //registration of UnitOfWork
            builder.RegisterType<DapperUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();

            //Register IDbConnection
            string connectionString = ConfigurationManager.ConnectionStrings["SubscriptionConnectionString"].ConnectionString;

            builder.RegisterType<SqlConnection>().As<IDbConnection>().UsingConstructor(typeof(string))
                .WithParameter("connectionString", connectionString).AsImplementedInterfaces().ExternallyOwned();

            builder.Register(c => RabbitHutch.CreateBus().Advanced).As<IAdvancedBus>().InstancePerLifetimeScope();
        }
    }

    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //Register WebAPi controllers
            builder.RegisterApiControllers(ThisAssembly);
            HttpConfiguration httpConfiguration = new HttpConfiguration();

            httpConfiguration.MapHttpAttributeRoutes();
            httpConfiguration.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            builder.RegisterInstance(httpConfiguration).AsSelf();
        }
    }
}
