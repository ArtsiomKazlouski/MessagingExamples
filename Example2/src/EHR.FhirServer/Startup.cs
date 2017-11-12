using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using EHR.FhirServer.Config;
using EHR.FhirServer.Core;
using EHR.FhirServer.Exceptions;
using EHR.FhirServer.Formatters.Input;
using EHR.FhirServer.Formatters.Output;
using EHR.FhirServer.Infrastructure;
using EHR.FhirServer.Infrastructure.MeditR;
using EHR.FhirServer.Infrastructure.WebApi;
using EHR.FhirServer.Operations;
using EHR.ServerEvent.Infrastructure;
using FluentValidation;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;




namespace EHR.FhirServer
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get;  set; }
        private readonly Toggles _toggles = new Toggles();
        
     
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            Configuration.GetSection("Toggles").Bind(_toggles);
            
            var mvcCoreBuilder = services.AddMvcCore(opt =>
                    {
                      
                       
                        opt.RespectBrowserAcceptHeader = true; // false by default
                        
                        opt.InputFormatters.Clear();
                        opt.InputFormatters.Add(new FhirJsonMediaTypeInputFormatter());
                        opt.InputFormatters.Add(new FhirXmlMediaTypeInputFormatter());
                      
                        opt.OutputFormatters.Clear();
                        opt.OutputFormatters.Add(new FhirJsonMediaTypeOutputFormatter());
                        opt.OutputFormatters.Add(new FhirXmlMediaTypeOutputFormatter());
                      
                    }
                )
                .AddFormatterMappings(map =>
                {
                    map.SetMediaTypeMappingForFormat(ContentType.FORMAT_PARAM_XML, ContentType.XML_CONTENT_HEADER);
                    map.SetMediaTypeMappingForFormat(ContentType.FORMAT_PARAM_JSON, ContentType.JSON_CONTENT_HEADER);
                })
                ;

            //if (_toggles.Authentication)
            //{
            //    mvcCoreBuilder.AddScopeRequirementPolicy("EHR");
            //    services.Configure<Endpoints>(Configuration.GetSection("Endpoints"));
            //}
          
         
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Startup))
                .AddClasses(filter => filter.AssignableTo<IValidator>())
                .AsImplementedInterfaces());

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Startup))
                .AddClasses(filter => filter.AssignableTo<IInputFormatter>())
                .AsImplementedInterfaces());

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Startup))
                .AddClasses(filter => filter.AssignableTo<IOutputFormatter>())
                .AsImplementedInterfaces());

         
            services.AddTransient<OperationEngine, OperationEngine>();

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Startup))
                .AddClasses(filter => filter.AssignableTo<IOperationHandler>())
                .AsImplementedInterfaces());


            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            services
                .Decorate(typeof(IRequestHandler<,>), typeof(FluentValidationRequestHandler<,>));

            var connectionString = Configuration.GetConnectionString("prescription_db");

            services.UseMediaTypeHeaderEnricher();

            services.AddTransient<Func<NpgsqlConnection>>(provider => ()=> new NpgsqlConnection(connectionString));
            services.AddScoped<IFhirDataBaseProvider, FhirDataBaseProvider>();
            services.AddTransient<IFhirBase, FhirBase>();
            services.AddSingleton(provider =>
            {
                using (var fhirDataProvider = new FhirDataBaseProvider(() => new NpgsqlConnection(connectionString)))
                {
                    var fhirBase = new FhirBase(fhirDataProvider);
                    return (Conformance)fhirBase.Conformance("UIIP", "1.2.0", "0.5.0", false);
                }
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
           
            if (_toggles.ServerEvent)
            {
                app.UseServerEventHandler();
            }
            app.UseUnitOfWorkTransactionMiddleware();

            app.UseHttpStatusToOperationOutcomeMiddleware(options => ConfigureHttpStatusToOperationOutcomeMiddleware(app, options))
                .UseExceptionHandler(new ExceptionHandlerOptions(){ExceptionHandler = context => Task.CompletedTask});

            if (_toggles.Authentication)
            {
                app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
                {

                    Authority = app.ApplicationServices.GetService<IOptions<Endpoints>>().Value.Authority,
                    ApiName = "EHR",
                    RequireHttpsMetadata = false,

                    AutomaticAuthenticate = true,

                });
            }

            app.UseMediaTypeHeaderEnricher()
                .UseMediaTypeValidator();

            app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().WithMethods(HttpMethods.Get.ToString()));
            //for Nginx
            //https://docs.microsoft.com/en-us/aspnet/core/publishing/linuxproduction?tabs=aspnetcore1x
            if (_toggles.Nginx)
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
          
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
            }
          
            app.UseMvc();
          
        }





        private static void ConfigureHttpStatusToOperationOutcomeMiddleware(IApplicationBuilder applicationBuilder, HttpStatusToOperationOutcomeMiddlewareOptions options)
        {
            var availableAccept = string.Join(", ", applicationBuilder.ApplicationServices.GetServices<IOutputFormatter>().OfType<TextOutputFormatter>().SelectMany(e => e.SupportedMediaTypes).Select(t => $"\"{t}\""));
            var availableContentType = string.Join(", ", applicationBuilder.ApplicationServices.GetServices<IInputFormatter>().OfType<TextInputFormatter>().SelectMany(e => e.SupportedMediaTypes).Select(t => $"\"{t}\""));


            var acceptErrorMesage = $"Не верные параметры запроса. Сервер поддерживает форматы: {availableAccept}. Сервер поддерживает поддерживает только кодировку UTF-8";
            var contentTypeErrorMesage = $"Не верные параметры запроса. Сервер поддерживает кодировку UTF-8 и форматы: {availableContentType}.";

            options.ForException<FhirHttpResponseException>().UseHandler(exception =>new OperationOutcomeResultContext(exception.StatusCode, exception.OperationOutcome));
            options.ForException<ValidationException>().UseHandler(exception =>new OperationOutcomeResultContext((HttpStatusCode) 400,OperationOutcomeExtensions.GetOperationOutcome().AddFailures(exception.Errors).AddException(exception)));
            options.ForException<System.ComponentModel.DataAnnotations.ValidationException>().UseHandler(exception =>new OperationOutcomeResultContext((HttpStatusCode) 400,OperationOutcomeExtensions.GetOperationOutcome().AddError(exception.ValidationResult.ErrorMessage)));
            options.ForException<FormatException>().UseHandler(exception =>new OperationOutcomeResultContext((HttpStatusCode) 422,OperationOutcomeExtensions.GetOperationOutcome().AddError("Невозможно выполнить синтаксический разбор ресурса, либо он не проходит базовые правила валидации FHIR").AddException(exception)));
            options.ForStatusCode(HttpStatusCode.NotAcceptable).UseHandler(code =>new OperationOutcomeResultContext(code,OperationOutcomeExtensions.GetOperationOutcome().AddError(acceptErrorMesage)));
            options.ForStatusCode(HttpStatusCode.UnsupportedMediaType).UseHandler(code =>new OperationOutcomeResultContext(code,OperationOutcomeExtensions.GetOperationOutcome().AddError(contentTypeErrorMesage)));
            options.ForStatusCode(HttpStatusCode.Forbidden).UseHandler(code => new OperationOutcomeResultContext(code,OperationOutcomeExtensions.GetOperationOutcome().AddError("Нет прав на доступ к запрашиваемому ресурсу")));
            options.ForStatusCode(HttpStatusCode.Unauthorized).UseHandler(code => new OperationOutcomeResultContext(code,OperationOutcomeExtensions.GetOperationOutcome().AddError("Необходимо пройти аутентификацию")));




        }





    }

}

