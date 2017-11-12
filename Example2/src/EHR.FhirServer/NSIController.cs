using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EHR.FhirServer.Infrastructure.WebApi;
using EHR.FhirServer.Interaction.Read;
using EHR.FhirServer.Interaction.Search;
using EHR.FhirServer.Interaction.Vread;
using Hl7.Fhir.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EHR.FhirServer
{
    [Route("fhir", Order = 0)]
    [FormatFilter]
    public class NSIController : Controller
    {

        private readonly IMediator _mediator;
       
        public NSIController(IMediator mediator, Conformance conformance)
        {
            _mediator = mediator;
        }

       

        [HttpGet]
        [Route("Organization/{id}")]
        public async Task<IActionResult> Read(string id)
        {
           return new FhirResponseResult(HttpStatusCode.OK, await _mediator.Send(new ReadRequest(ResourceType.Organization.ToString(), id)));
        }

       

        [HttpGet, Route("Organization/{id}/_history/{vid}")]
        public async Task<IActionResult> VRead(string id, string vid)
        {
            return new FhirResponseResult(HttpStatusCode.OK, await _mediator.Send(new VreadRequest(ResourceType.Organization.ToString(), id, vid)));
        }
        
        [HttpGet, Route("Organization")]
        public async Task<IActionResult> Search()
        {
            var nameValuesPairs = Request.Query.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.ToString())).ToList();
            return new FhirResponseResult(HttpStatusCode.OK, await _mediator.Send(new SearchRequest(ResourceType.Organization.ToString(), nameValuesPairs)));
        }
    }
}
