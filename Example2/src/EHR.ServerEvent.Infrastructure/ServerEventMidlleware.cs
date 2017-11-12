using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EHR.FhirServer.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace EHR.ServerEvent.Infrastructure
{
    public static class ServerEventMidllewareExtensions
    {
        public static IApplicationBuilder UseServerEventHandler(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServerEventMidlleware>();
        }
    }


    internal class ServerEventMidlleware
    {
        private readonly RequestDelegate _next;

        public ServerEventMidlleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context, IFhirDataBaseProvider dataBaseProvider)
        {
            if (!context.Request.Path.Value.Contains("/fhir/"))
            {
                await _next(context);
                return;
            }

            using (var requestBodyStream = new MemoryStream())
            {
                using (var responseBodyStream = new MemoryStream())
                {
                    var originalRequestBody = context.Request.Body;

                    var originalResponseBody = context.Response.Body;
                    var requestBodyText = "";
                    var responseBodyText = "";

                    await context.Request.Body.CopyToAsync(requestBodyStream);
                    requestBodyStream.Seek(0, SeekOrigin.Begin);

                    requestBodyText = Convert.ToBase64String(requestBodyStream.ToArray());

                    requestBodyStream.Seek(0, SeekOrigin.Begin);
                    context.Request.Body = requestBodyStream;


                    context.Response.Body = responseBodyStream;


                    await _next(context);


                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    responseBodyText = Convert.ToBase64String(responseBodyStream.ToArray());

                    responseBodyStream.Seek(0, SeekOrigin.Begin);

                    await responseBodyStream.CopyToAsync(originalResponseBody);

                    

                    dataBaseProvider.WithConnection<object>("server_event.create_server_event", new NpgsqlParameter
                    {
                        NpgsqlDbType = NpgsqlDbType.Jsonb,
                        Value = JsonConvert.SerializeObject(
                            BuildActionMetadata(context, responseBodyText, requestBodyText))
                    });

                    context.Request.Body = originalRequestBody;
                    context.Response.Body = originalResponseBody;
                }
            }
        }


        private ActionMetadata BuildActionMetadata(HttpContext context, string responceBody, string requestBody)
        {
            return new ActionMetadata
            {
                ExecutionDateTime = DateTime.UtcNow,
                ActionResponce = new ActionResponce
                {
                    Payload = responceBody,
                    StatusCode = context.Response.StatusCode,
                    Header = context.Response.Headers.Select(h => new KeyValuePair<string, string>(h.Key, h.Value))
                },
                ActionRequest = new ActionRequest
                {
                    RouteData = context.GetRouteData()?.Values,
                    Payload = requestBody,
                    Header = context.Request.Headers.Select(h => new KeyValuePair<string, string>(h.Key, h.Value)),
                    HttpMethod = context.Request.Method,
                    Query = context.Request.QueryString.ToString(),
                    URI = context.Request.Path
                }
            };
        }
    }
}