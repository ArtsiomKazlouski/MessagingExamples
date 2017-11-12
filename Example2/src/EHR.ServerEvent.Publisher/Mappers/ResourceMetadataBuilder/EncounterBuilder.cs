using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class EncounterBuilder:BaseResourceInformationBuilder<Encounter>
    {
        protected override ResourceInfo GetRelatedEncounter(Encounter resource)
        {
           return new ResourceInfo(){ResourceId = resource.Id, ResourceVersionId = resource.VersionId};
        }

        protected override ResourceInfo GetRelatedPatientInformation(Encounter resource)
        {
            return new ResourceInfo() { ResourceId = resource.Patient?.Reference};
        }
    }
}
