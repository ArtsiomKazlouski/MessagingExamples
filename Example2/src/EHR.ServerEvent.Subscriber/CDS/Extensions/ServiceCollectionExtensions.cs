using System;
using System.Data;
using System.Reflection;
using EHR.Cds.Infrastructure;
using EHR.ServerEvent.Subscriber.Cds.Handlers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Scrutor;

namespace EHR.ServerEvent.Subscriber.CDS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseCds(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<Func<IDbConnection>>(() => new NpgsqlConnection(configuration.GetConnectionString("CdsDatabaseConnectionString")));
            services.AddTransient<IUnitOfWork, DapperUnitOfWork>();
            services.AddSingleton<Func<IUnitOfWork>>(sp => (() => sp.GetService<IUnitOfWork>()));
            services.Scan(scan => scan
                .FromAssemblyOf<IConsumerHandler>()
                .AddClasses(classess => classess.AssignableTo<IConsumerHandler>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            return services;
        }
    }
}
