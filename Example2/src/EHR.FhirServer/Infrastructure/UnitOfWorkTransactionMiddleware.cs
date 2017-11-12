using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EHR.FhirServer.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EHR.FhirServer.Infrastructure
{
    public static class UnitOfWorkTransactionMiddlewareExtentions
    {


        public static IApplicationBuilder UseUnitOfWorkTransactionMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<UnitOfWorkTransactionMiddleware>();
            return applicationBuilder;
        }
    }


    public class UnitOfWorkTransactionMiddleware
    {
        private readonly RequestDelegate _next;
      
        public UnitOfWorkTransactionMiddleware(RequestDelegate next)
        {
            _next = next;
          
        }

        public async Task Invoke(HttpContext context, IFhirDataBaseProvider dataBaseProvider)
        {
           
            await _next.Invoke(context);

            if (context.Response.IsSuccessStatusCode())
                   return;

            dataBaseProvider.PeventCommit();
        }
    }
}
