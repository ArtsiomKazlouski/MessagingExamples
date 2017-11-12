using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class CompositionBuilder:BaseResourceInformationBuilder<Composition>
    {
        protected override ResourceInfo GetRelatedPatientInformation(Composition resource)
        {
            return new ResourceInfo(){ResourceId = resource.Subject?.Reference};
        }
    }
}
