using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using EHR.Cds.Hooks;
using EHR.Cds.Host.Config;
using EHR.Cds.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Npgsql;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EHR.Cds.Host
{
    public class Startup
    {
        private readonly Toggles _toggles = new Toggles();
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            HostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            Configuration.GetSection("Toggles").Bind(_toggles);
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new ResourceCustomJsonConverter(),
                }
            };



            services.AddTransient<Func<IDbConnection>>(provider => () =>
                new NpgsqlConnection(Configuration.GetConnectionString("DatabaseConnectionString")));
            services.AddTransient<Func<NpgsqlConnection>>(provider => () =>
                new NpgsqlConnection(Configuration.GetConnectionString("DatabaseConnectionString")));
            services.AddScoped<IUnitOfWork, DapperNonTransactionalUnitOfWork>();
         
            services.UseCdsHooks(Configuration.GetSection("CdsHooksSettings"));

            services.AddMvcCore().AddJsonFormatters(settings => { settings.Converters.Add(new ResourceCustomJsonConverter()); }).AddApiExplorer();

            services.AddMvcCore().AddApiExplorer();
            ConfigureDocumentation(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //for Nginx
            //https://docs.microsoft.com/en-us/aspnet/core/publishing/linuxproduction?tabs=aspnetcore1x
            if (_toggles.Nginx)
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
            app.UseMvcWithDefaultRoute();

            ConfigureDocumentation(app);
        }

        protected virtual void ConfigureDocumentation(IServiceCollection services)
        {
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Cds Disability Sheet Host", Version = "v1" });
                c.DescribeAllEnumsAsStrings();

                foreach (var file in DefaultGetCommentFilesPathFunction($@"{HostingEnvironment.ContentRootPath}", "*.xml"))
                {
                    c.IncludeXmlComments(file);
                }

            });
        }

        protected virtual void ConfigureDocumentation(IApplicationBuilder app)
        {
            


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cds Disability Sheet Host");
              
            });
        }


        private static IEnumerable<string> DefaultGetCommentFilesPathFunction(string path, string mask)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            return Directory.GetFiles(path, mask);
        }

      
    }
}
