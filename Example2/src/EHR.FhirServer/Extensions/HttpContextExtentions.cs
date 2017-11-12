using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EHR.FhirServer.Extensions
{
    public static class HttpContextExtentions
    {
        public static bool IsSuccessStatusCode(this HttpResponse response)
        {
            return (response.StatusCode >= 200) && (response.StatusCode <= 299);

        }
    }
}
