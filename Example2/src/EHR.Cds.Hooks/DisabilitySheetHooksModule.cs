using System;
using EHR.Cds.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace EHR.Cds.Hooks
{
    public static class CdsHookExtentions
    {

        public static IServiceCollection UseCdsHooks(this IServiceCollection serviceCollection, IConfigurationSection configurationSection)
        {
           
            serviceCollection.AddOptions();

           
            serviceCollection.Configure<DisabilitySheetCdsSettings>(configurationSection);

            serviceCollection.AddTransient<ICdsService, DisabilitySheetService>();
            serviceCollection.Scan(scan => scan
                .FromAssemblyOf<DisabilitySheetHandlerBase>()
                .AddClasses(classes => classes.AssignableTo<IRecommendationHandler>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());
          
            return serviceCollection;
        }
    }
}