﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Calculator.WebApi.Startup))]

namespace Calculator.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<WebApiModule>();
            
            ILifetimeScope container = builder.Build();

            

            HttpConfiguration httpConfiguration = container.Resolve<HttpConfiguration>();
            //ConFigureDocumentations(httpConfiguration);

            app
                .UseAutofacMiddleware(container)
                .UseAutofacWebApi(httpConfiguration)
                .UseWebApi(httpConfiguration);
        }
    }
}
