using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;


namespace EHR.FhirServer.Infrastructure
{
    public static class MediaTypeValidatorMiddlewareExtentions
    {
       

        public static IApplicationBuilder UseMediaTypeValidator(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<MediaTypeValidatorMiddleware>();
            return applicationBuilder;
        }
    }


    public class MediaTypeValidatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IInputFormatter> _inputFormatters;
        private readonly IEnumerable<IOutputFormatter> _outputFormatters;

        public MediaTypeValidatorMiddleware(RequestDelegate next, IEnumerable<IInputFormatter> inputFormatters, IEnumerable<IOutputFormatter> outputFormatters)
        {
            _next = next;
            _inputFormatters = inputFormatters;
            _outputFormatters = outputFormatters;
        }


        public async Task Invoke(HttpContext context)
        {
           var contentType = context.Request.GetTypedHeaders().ContentType;
           var accept = context.Request.GetTypedHeaders().Accept;
           var acceptCharset = context.Request.GetTypedHeaders().AcceptCharset;



            if (contentType != null)
            {
                if (_inputFormatters.OfType<TextInputFormatter>()
                        .Any(forrmatter =>
                            forrmatter.SupportedMediaTypes.Any(m => m.Equals(contentType.MediaType)
                                                                    && (contentType.Encoding ==null || forrmatter.SupportedEncodings.Any(e => e.WebName.Equals(contentType.Encoding.WebName)))  )) == false)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                    return;
                }
            }

           

            

            if (_outputFormatters.OfType<TextOutputFormatter>()
                    .Any(forrmatter =>
                        forrmatter.SupportedMediaTypes.Any(m => accept==null || accept.Any(a => a.MediaType.Equals(m))
                     && forrmatter.SupportedEncodings.Any(e => acceptCharset==null ||acceptCharset.Any(a =>a.Value.Equals(e.WebName))))) == false)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return;
            }


            await _next(context);
          

          
        }



      
    }



  
}
