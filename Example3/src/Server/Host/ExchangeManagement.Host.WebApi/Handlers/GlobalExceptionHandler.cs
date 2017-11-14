using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using ExchangeManagement.Host.WebApi.Exceptions;
using FluentValidation;

namespace ExchangeManagement.Host.WebApi.Handlers
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        private readonly Dictionary<Type, HttpStatusCode> _errorsDictionary = new Dictionary<Type, HttpStatusCode>
        {
            { typeof(EntityNotFoundException), HttpStatusCode.NotFound},
            { typeof(EntityUpdateConcurrencyException), HttpStatusCode.Conflict},
            { typeof(ValidationException), HttpStatusCode.BadRequest},
        };

        public override void Handle(ExceptionHandlerContext context)
        {
            base.Handle(context);
            var errorType = context.Exception.GetType();
            var error = _errorsDictionary.Where(s => s.Key == errorType)
                .Select(e => (KeyValuePair<Type, HttpStatusCode>?)e)
                .FirstOrDefault();

            var validationException = context.Exception as ValidationException;
            var errorMessage = validationException != null ? string.Join(Environment.NewLine, validationException.Errors.Select(e => e.ErrorMessage)) : context.Exception.Message;

            var statusCode = error?.Value ?? HttpStatusCode.InternalServerError;
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(errorMessage),
                ReasonPhrase = error?.Key.ToString() ?? context.Exception.GetType().ToString()
            };
            context.Result = new ErrorMessageResult(context.Request, response);
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