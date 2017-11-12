using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class PatientBuilder : BaseResourceInformationBuilder<Patient>
    {
        protected override ResourceInfo GetRelatedPatientInformation(Patient resource)
        {
            return new ResourceInfo(){ResourceId = resource.Id, ResourceVersionId = resource.Id};
        }
    }
}
