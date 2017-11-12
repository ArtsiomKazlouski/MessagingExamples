using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EHR.FhirServer.Exceptions;
using EHR.FhirServer.Extensions;
using EHR.FhirServer.Infrastructure.WebApi;
using FluentValidation;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationOutcomeExtensions = EHR.FhirServer.Infrastructure.WebApi.OperationOutcomeExtensions;

namespace EHR.FhirServer.Infrastructure
{
    public static class HttpStatusToOperationOutcomeMiddlewareExtentions
    {
        public static IApplicationBuilder UseHttpStatusToOperationOutcomeMiddleware(this IApplicationBuilder applicationBuilder, Action<HttpStatusToOperationOutcomeMiddlewareOptions> optionsAction)
        {
            var options = new HttpStatusToOperationOutcomeMiddlewareOptions();
            optionsAction.Invoke(options);
            applicationBuilder.UseMiddleware<HttpStatusToOperationOutcomeMiddleware>(Options.Create(options));
            return applicationBuilder;
        }
    }


    




    public class HttpStatusToOperationOutcomeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MvcOptions _options;
        private readonly HttpStatusToOperationOutcomeMiddlewareOptions _httpStatusToOperationOutcomeMiddlewareOptions;



        public HttpStatusToOperationOutcomeMiddleware(RequestDelegate next, IOptions<MvcOptions> options, IOptions<HttpStatusToOperationOutcomeMiddlewareOptions> httpStatusToOperationOutcomeMiddlewareOptions)
        {
            _next = next;
            _options = options.Value;
            _httpStatusToOperationOutcomeMiddlewareOptions = httpStatusToOperationOutcomeMiddlewareOptions?.Value ?? new HttpStatusToOperationOutcomeMiddlewareOptions();
        }

        public async Task Invoke(HttpContext context, IHttpResponseStreamWriterFactory writerFactory)
        {
            await _next(context);

            if (context.Response.IsSuccessStatusCode())
                return;


            var result = new OperationOutcomeResultContext(HttpStatusCode.InternalServerError, OperationOutcomeExtensions.GetOperationOutcome().AddError("Внутренняя ошибка сервера"));

            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature != null)
            {
                 result = _httpStatusToOperationOutcomeMiddlewareOptions.GetExceptionHandler(exceptionHandlerFeature.Error)?
                    .Invoke(exceptionHandlerFeature.Error) ?? result;
             
            }


            result = _httpStatusToOperationOutcomeMiddlewareOptions.GetStatusHandler((HttpStatusCode)context.Response.StatusCode)?
                .Invoke((HttpStatusCode)context.Response.StatusCode) ?? result;
           


            var outputFormatter = (context.Features.Get<IOutputFormatter>() ?? _options.OutputFormatters.First());

            await result.ExecuteResultAsync(context, outputFormatter, writerFactory);
            
        }


      
    }



    public class HttpStatusToOperationOutcomeMiddlewareOptions
    {

        public HttpStatusToOperationOutcomeMiddlewareOptions()
        {
            _statusHandlers = new Dictionary<HttpStatusCode, IHttpStatusToOperationOutcomeMiddlewareStatusHandler>();
            _exceptionHandlers = new Dictionary<Type, IHttpStatusToOperationOutcomeMiddlewareExceptionHandler>();
        }

        private readonly Dictionary<HttpStatusCode, IHttpStatusToOperationOutcomeMiddlewareStatusHandler> _statusHandlers;
        private readonly Dictionary<Type,IHttpStatusToOperationOutcomeMiddlewareExceptionHandler> _exceptionHandlers;


        public Func<HttpStatusCode, OperationOutcomeResultContext> GetStatusHandler(HttpStatusCode code)
        {
            _statusHandlers.TryGetValue(code, out var statusHandler);
            return statusHandler?.GetStatusHandler();
        }

        public Func<Exception, OperationOutcomeResultContext> GetExceptionHandler(Exception exception)
        {
            if (exception == null)
                return null;
            _exceptionHandlers.TryGetValue(exception.GetType(), out var exceptionHandler);
            return exceptionHandler?.GetExceptioenHandler();
          
        }

        public HttpStatusToOperationOutcomeMiddlewareExceptionHandler<T> ForException<T>() where T:Exception
        {
            return new HttpStatusToOperationOutcomeMiddlewareExceptionHandler<T>(_exceptionHandlers);
        }

        public HttpStatusToOperationOutcomeMiddlewareStatusHandler ForStatusCode(HttpStatusCode httpStatusCode)
        {
            return new HttpStatusToOperationOutcomeMiddlewareStatusHandler(_statusHandlers, httpStatusCode);
        }


       
    }

    public interface IHttpStatusToOperationOutcomeMiddlewareStatusHandler
    {
       
        Func<HttpStatusCode, OperationOutcomeResultContext> GetStatusHandler();

    }


    public class HttpStatusToOperationOutcomeMiddlewareStatusHandler : IHttpStatusToOperationOutcomeMiddlewareStatusHandler
    {
        public HttpStatusToOperationOutcomeMiddlewareStatusHandler(Dictionary<HttpStatusCode, IHttpStatusToOperationOutcomeMiddlewareStatusHandler>  handlers, HttpStatusCode httpStatusCode)
        {
            SupportedCode = httpStatusCode;
            _handlers = handlers;
        }

        private readonly Dictionary<HttpStatusCode, IHttpStatusToOperationOutcomeMiddlewareStatusHandler> _handlers;

        private Func<HttpStatusCode, OperationOutcomeResultContext> _handler;

        public HttpStatusCode SupportedCode { get; set; }

      
        public Func<HttpStatusCode, OperationOutcomeResultContext> GetStatusHandler()
        {
            return _handler;
        }


        public void UseHandler(Func<HttpStatusCode, OperationOutcomeResultContext> statusHandler)
        {
            _handler = statusHandler;
            _handlers.Add(SupportedCode, this);
           
        }
    }

    public interface IHttpStatusToOperationOutcomeMiddlewareExceptionHandler
    {
        
        Func<Exception, OperationOutcomeResultContext> GetExceptioenHandler();

    }

    public class HttpStatusToOperationOutcomeMiddlewareExceptionHandler<T> : IHttpStatusToOperationOutcomeMiddlewareExceptionHandler where T : Exception
    {
        public HttpStatusToOperationOutcomeMiddlewareExceptionHandler(Dictionary<Type, IHttpStatusToOperationOutcomeMiddlewareExceptionHandler> handlers)
        {
            _handlers = handlers;
        }

        private readonly Dictionary<Type, IHttpStatusToOperationOutcomeMiddlewareExceptionHandler> _handlers;

        private Func<T, OperationOutcomeResultContext> _handler;

        public Func<Exception, OperationOutcomeResultContext> GetExceptioenHandler()
        {
            return exception => _handler(exception as T);
        }
        

        public void UseHandler(Func<T, OperationOutcomeResultContext> exceptionHandler)
        {
            _handler = exceptionHandler;
            _handlers.Add(typeof(T), this);
        }

    }


    public class OperationOutcomeResultContext
    {
        public OperationOutcomeResultContext(HttpStatusCode httpStatusCode, OperationOutcome operationOutcome)
        {
            _httpStatusCode = httpStatusCode;
            _operationOutcome = operationOutcome;
        }



        private readonly HttpStatusCode _httpStatusCode;

        private readonly OperationOutcome _operationOutcome;



        public async Task ExecuteResultAsync(HttpContext context, IOutputFormatter formatter, IHttpResponseStreamWriterFactory httpResponseStreamWriterFactory)
        {
            context.Response.StatusCode = (int)_httpStatusCode;

            var forrmatterContext = new OutputFormatterWriteContext(context,
                httpResponseStreamWriterFactory.CreateWriter,
                typeof(OperationOutcome), _operationOutcome);

            await formatter.WriteAsync(forrmatterContext);
        }
    }
}