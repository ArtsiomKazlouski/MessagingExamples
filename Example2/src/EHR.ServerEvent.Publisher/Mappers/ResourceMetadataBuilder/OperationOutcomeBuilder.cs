using System;
using System.Collections.Generic;
using System.Text;
using EHR.ServerEvent.Infrastructure;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class OperationOutcomeBuilder : BaseResourceInformationBuilder<OperationOutcome>
    {
        protected override IEnumerable<ServerEventResourceInfromation> BuildCore(OperationOutcome resource, ActionMetadata actionMetadata)
        {
            object type = "";
            actionMetadata.ActionRequest.RouteData.TryGetValue("type", out type);

            object id = "";
            actionMetadata.ActionRequest.RouteData.TryGetValue("id", out id);
            yield return new ServerEventResourceInfromation() { ResourceId = id?.ToString(), ResourceType = type?.ToString() };
        }
    }
}
