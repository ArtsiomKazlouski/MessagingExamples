using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EHR.FhirServer.Exceptions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

namespace EHR.FhirServer.Formatters.Input
{
    public abstract class BaseFhirInputFormatter : TextInputFormatter
    {
        protected BaseFhirInputFormatter(IEnumerable<MediaTypeHeaderValue> mediaTypeHeaderValues)
        {
            SupportedEncodings.Clear();
            SupportedEncodings.Add(Encoding.UTF8);

            SupportedMediaTypes.Clear();

            foreach (var mediaTypeHeaderValue in mediaTypeHeaderValues)
                SupportedMediaTypes.Add(mediaTypeHeaderValue);
        }

        protected override bool CanReadType(Type type)
        {
            return typeof(Resource).IsAssignableFrom(type);
        }

        protected abstract Resource Deserialize(Type type, string content);

        

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
            Encoding encoding)
        {
            

            using (var reader = new StreamReader(context.HttpContext.Request.Body, encoding, true))
            {
                var resource = Deserialize(context.ModelType, await reader.ReadToEndAsync());

                DotNetAttributeValidation.Validate(resource, true);

                return InputFormatterResult.Success(resource);
            }
        }

       
    }
}