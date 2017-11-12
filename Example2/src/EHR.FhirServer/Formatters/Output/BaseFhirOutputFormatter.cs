using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace EHR.FhirServer.Formatters.Output
{
    public abstract class BaseFhirOutputFormatter : TextOutputFormatter
    {
      
        protected BaseFhirOutputFormatter(IEnumerable<MediaTypeHeaderValue> mediaTypeHeaderValues)
        {
            SupportedEncodings.Clear();
            SupportedEncodings.Add(new UTF8Encoding(true));
            SupportedMediaTypes.Clear();
            foreach (var mediaTypeHeaderValue in mediaTypeHeaderValues)
                SupportedMediaTypes.Add(mediaTypeHeaderValue);
        }


        protected override bool CanWriteType(Type type)
        {
            return typeof(Resource).IsAssignableFrom(type);
        }


      
  


        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context,
            Encoding selectedEncoding)
        {

           
            if (context == null)
                return;

          
            var response = context.HttpContext.Response;

            using (var delegatingStream = new NonDisposableStream(response.Body))
            using (var streamWriter = new StreamWriter(delegatingStream, selectedEncoding, 1024,true))
            {
                Serialize(context.ObjectType, context.Object as Resource, streamWriter, selectedEncoding);
                await streamWriter.FlushAsync();
            }

         
        }


        protected abstract void Serialize(Type type, Resource value, TextWriter writer, Encoding encoding);
    }
}