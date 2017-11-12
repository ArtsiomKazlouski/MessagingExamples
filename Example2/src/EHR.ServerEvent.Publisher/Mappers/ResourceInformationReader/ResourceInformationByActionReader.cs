using System;
using System.Collections.Generic;
using System.Linq;
using EHR.ServerEvent.Infrastructure;
using EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceInformationReader
{
    /// <summary>
    /// Стратегия получения ServerEventResourceInfromation из массива хранящегосяв бд на основе HttpAction
    /// </summary>
    public abstract class ResourceInformationReader
    {
        protected readonly ResourceBuilderFuctory BuildersFuctory;
        protected readonly IEnumerable<PayloadReader.PayloadReader> PayloadReaders;

        protected ResourceInformationReader(ResourceBuilderFuctory buildersFuctory, IEnumerable<PayloadReader.PayloadReader> payloadReaders, string httpMethod)
        {
            BuildersFuctory = buildersFuctory;
            PayloadReaders = payloadReaders;
            HttpMethod = httpMethod;
        }

        protected string HttpMethod { get; }

        public virtual bool CanRead(string httpMethod)
        {

            return string.Equals(HttpMethod, httpMethod, StringComparison.CurrentCultureIgnoreCase);
        }


        public virtual IEnumerable<ServerEventResourceInfromation> ResourceInformation(ActionMetadata actionMetadata)
        {

            var resource = ReadPayloadFromResponce(actionMetadata);

            return BuildersFuctory.GetBuilder(resource).Build(resource, actionMetadata);

        }
       
        private Func<IEnumerable<KeyValuePair<string, string>>, PayloadReader.PayloadReader> PauloadFactory => headers =>
        {
            var payloadReaders = PayloadReaders.FirstOrDefault(p => p.CanRead(headers));
            if (payloadReaders == null)
            {
                throw new NotSupportedException($"Unsupported content type {headers.FirstOrDefault(x=>x.Key=="Content-Type").Value}.");
            }
            return payloadReaders;
        };


        protected Func<ActionMetadata, Resource> ReadPayloadFromRequest => actionMetadata => PauloadFactory(actionMetadata.ActionRequest.Header).Read(actionMetadata.ActionRequest.Payload);
        protected Func<ActionMetadata, Resource> ReadPayloadFromResponce => actionMetadata => PauloadFactory(actionMetadata.ActionResponce.Header).Read(actionMetadata.ActionResponce.Payload);


    }
}
