using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class EpisodeOfCareBuilder:BaseResourceInformationBuilder<EpisodeOfCare>
    {
        protected override ResourceInfo GetRelatedPatientInformation(EpisodeOfCare resource)
        {
            return new ResourceInfo() {ResourceId = resource.Patient?.Reference};
        }
    }
}
