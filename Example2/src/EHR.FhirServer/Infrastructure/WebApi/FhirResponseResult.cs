using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EHR.FhirServer.Formatters;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace EHR.FhirServer.Infrastructure.WebApi
{
    public class FhirResponseResult : ObjectResult
    {

        protected Resource Resource;

        public FhirResponseResult(HttpStatusCode statusCode, Resource resource) 
            : base(resource)
        {
            Resource = resource;
            StatusCode = (int)statusCode;
        }


     


        public override Task ExecuteResultAsync(ActionContext context)
        {
          
          
            if (Value == null)
            {
                 Task.FromResult(0);
            }
            context.HttpContext.Response.StatusCode = (int) StatusCode;

            //TODO refactoring extract method AcquireHeaders

            if (string.IsNullOrEmpty(Resource.Id) == false
                && string.IsNullOrEmpty(Resource.VersionId) == false)
            {
                var utlHelperFactory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
                
                Uri contentLocationUri = new Uri(utlHelperFactory.GetUrlHelper(context).Link("VRead", new Dictionary<string, object>
                {
                    {"type", Resource.ResourceType},
                    {"id", Resource.Id},
                    {"vid", Resource.VersionId},
                }));

                if (context.HttpContext.Request.Method == "POST"
                    || context.HttpContext.Request.Method == "PUT")
                {
                    context.HttpContext.Response.Headers.Add("Location", contentLocationUri.AbsolutePath);
                }
                else
                {
                    context.HttpContext.Response.Headers.Add("Content-Location",contentLocationUri.AbsolutePath);
                }

                context.HttpContext.Response.Headers.Add("ETag", new EntityTagHeaderValue(string.Format("\"{0}\"", Resource.VersionId), true).ToString());
            }

            if (Resource.Meta != null)
            {
                context.HttpContext.Response.Headers.Add("Last-Modified",Resource.Meta.LastUpdated?.ToString("R"));
            }
           
            return base.ExecuteResultAsync(context);
        }
    }
}