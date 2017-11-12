using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class RelatedPersonBuilder:BaseResourceInformationBuilder<RelatedPerson>
    {
        protected override ResourceInfo GetRelatedPatientInformation(RelatedPerson resource)
        {
           return new ResourceInfo(){ResourceId = resource.Patient?.Reference};
        }
    }
}
