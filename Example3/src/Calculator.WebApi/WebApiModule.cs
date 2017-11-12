using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Integration.WebApi;
using Serilog;

namespace Calculator.WebApi
{
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterApiControllers(ThisAssembly);

            HttpConfiguration httpConfiguration = new HttpConfiguration();
            httpConfiguration.MapHttpAttributeRoutes();
            httpConfiguration.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            builder.RegisterInstance(httpConfiguration).AsSelf();
        }
    }

    public class GlobalExceptionHandler : ExceptionHandler
    {

        static ILogger _logger;


        private static ILogger Logger => (_logger ?? Log.Logger).ForContext<GlobalExceptionHandler>();

        private readonly Dictionary<Type, HttpStatusCode> _errorsDictionary = new Dictionary<Type, HttpStatusCode>
        {
           
        };

        public override void Handle(ExceptionHandlerContext context)
        {

            base.Handle(context);
            var errorType = context.Exception.GetType();
            var error = _errorsDictionary.Where(s => s.Key == errorType)
                .Select(e => (KeyValuePair<Type, HttpStatusCode>?)e)
                .FirstOrDefault();
            
            var errorMessage =  context.Exception.Message;

            var stausCode = error?.Value ?? HttpStatusCode.InternalServerError;
            var resp = new HttpResponseMessage(stausCode)
            {
                Content = new StringContent(errorMessage),
                ReasonPhrase = error == null ? context.Exception.GetType().ToString() : error.Value.Key.ToString(),

            };
            context.Result = new ErrorMessageResult(context.Request, resp);

            if ((int)stausCode >= 500)
                Logger.Error(context.Exception, context.Exception.Message);
            else
                Logger.Information(context.Exception, context.Exception.Message);

        }

        public class ErrorMessageResult : IHttpActionResult
        {
            private HttpRequestMessage _request;
            private HttpResponseMessage _httpResponseMessage;


            public ErrorMessageResult(HttpRequestMessage request, HttpResponseMessage httpResponseMessage)
            {
                _request = request;
                _httpResponseMessage = httpResponseMessage;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_httpResponseMessage);
            }
        }
    }
}