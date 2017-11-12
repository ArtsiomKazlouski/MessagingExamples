using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EHR.FhirServer.Infrastructure.WebApi;
using EHR.FhirServer.Interaction.Create;
using EHR.FhirServer.Interaction.Delete;
using EHR.FhirServer.Interaction.Operation;
using EHR.FhirServer.Interaction.Read;
using EHR.FhirServer.Interaction.Search;
using EHR.FhirServer.Interaction.Update;
using EHR.FhirServer.Interaction.Vread;
using EHR.FhirServer.Operations;
using Hl7.Fhir.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHR.FhirServer
{
    [Route("fhir", Order = 1)]
    public class FhirController : Controller
    {
        private readonly IMediator _mediator;
        private readonly Conformance _conformance;

        public FhirController(IMediator mediator, Conformance conformance)
        {
            _mediator = mediator;
            _conformance = conformance;
          
        }

        [HttpPost, Route("{type}")]
        public async Task<IActionResult> Create(string type, [FromBody]Resource entry)
        {
            return FhirResponse(HttpStatusCode.Created, await _mediator.Send(new CreateRequest(type, entry)));
        }

        [HttpGet]
        [Route("{type}/{id}")]
        public async Task<IActionResult> Read(string type, string id)
        {
            return FhirResponse(HttpStatusCode.OK, await _mediator.Send(new ReadRequest(type, id)));
        }

        [HttpGet, Route("{type}/{id}/_history/{vid}", Name = "VRead")]
        public async Task<IActionResult> VRead(string type, string id, string vid)
        {
            return FhirResponse(HttpStatusCode.OK, await _mediator.Send(new VreadRequest(type, id, vid)));
        }

        [HttpPut, Route("{type}/{id}")]
        public async Task<IActionResult> Update(string type, string id, [FromBody]Resource entry)
        {
            return FhirResponse(HttpStatusCode.OK, await _mediator.Send(new UpdateRequest(type, id, entry)));
        }

        [HttpDelete, Route("{type}/{id}")]
        public async Task<IActionResult> Delete(string type, string id)
        {
            await _mediator.Send(new DeleteRequest(type, id));
            return NoContent();
        }

        [HttpGet, Route("{type}")]
        public async Task<IActionResult> Search(string type)
        {
            var nameValuesPairs = Request.Query.Select(kv=>new KeyValuePair<string, string>(kv.Key, kv.Value.ToString())).ToList();
            return FhirResponse(HttpStatusCode.OK, await _mediator.Send(new SearchRequest(type, nameValuesPairs)));
        }
         
        [AllowAnonymous]
        [HttpGet, Route("metadata")]
        public IActionResult Metadata()
        {
            return FhirResponse(HttpStatusCode.OK, _conformance);
        }
       

        [HttpGet, Route("{type}/${operationName}")]
        public async Task<IActionResult> TypeOperation(string type, string operationName)
        {
            var parameters = SubstractOperationParametersFromQuery(Request.QueryString.Value);
            return FhirResponse(HttpStatusCode.OK, await _mediator.Send(new OperationRequest(new OperationContext(operationName, parameters, type))));
        }

        [HttpPost, Route("{type}/${operationName}")]
        public async Task<IActionResult> TypeOperation(string type, string operationName, [FromBody]Parameters parameters)
        {
            parameters = MergeOperationParameters(parameters, SubstractOperationParametersFromQuery(Request.QueryString.Value));
            return FhirResponse(HttpStatusCode.OK, await _mediator.Send(new OperationRequest(new OperationContext(operationName, parameters, type))));
        }

        [HttpPost, Route("{type}/{id}/${operationName}")]
        public async Task<IActionResult> SpecificInstanceOperation(string type, string id, string operationName, Parameters parameters)
        {
            parameters = MergeOperationParameters(parameters, SubstractOperationParametersFromQuery(Request.QueryString.Value));
            return FhirResponse(HttpStatusCode.OK, await _mediator.Send(new OperationRequest(new OperationContext(operationName, parameters, type, id))));
        }

        private IActionResult FhirResponse(HttpStatusCode statusCode, Resource resource)
        {
            Response.StatusCode = (int) statusCode;
            return new FhirResponseResult(statusCode, resource);
        }



         private Parameters SubstractOperationParametersFromQuery(string query)
        {
            var parameters = new Parameters();
            List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrWhiteSpace(query) == false)
            {
              
                var nameValueCollection = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query);
                queryParams.AddRange(nameValueCollection.ToList().Select(c=>new KeyValuePair<string, string>(c.Key, c.Value.ToString())));
              
            }

            if (queryParams.Any()==false)
            {
                return null;
            }

            foreach (var keyValuePair in queryParams)
            {
                parameters.Add(keyValuePair.Key, new FhirString(keyValuePair.Value));
            }
            return parameters;
        }

        private Parameters MergeOperationParameters(Parameters first, Parameters second)
        {
            if (first==null&&second==null)
            {
                return null;
            }

            var resultParameters = new Parameters();
            
            if (first?.Parameter != null)
            {
                resultParameters.Parameter.AddRange(first.Parameter);
            }

            if (second?.Parameter!=null)
            {
                resultParameters.Parameter.AddRange(second.Parameter);
            }
            return resultParameters;
        }
    }
}