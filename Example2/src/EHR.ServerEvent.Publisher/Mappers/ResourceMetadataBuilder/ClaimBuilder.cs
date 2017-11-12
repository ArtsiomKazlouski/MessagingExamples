using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class ClaimBuilder:BaseResourceInformationBuilder<Claim>
    {
        protected override ResourceInfo GetRelatedPatientInformation(Claim resource)
        {
            return new ResourceInfo(){ResourceId = resource.Patient?.Reference};
        }
    }
}
