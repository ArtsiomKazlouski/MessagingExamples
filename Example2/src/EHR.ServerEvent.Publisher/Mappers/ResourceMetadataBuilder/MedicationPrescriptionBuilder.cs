using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public  class MedicationPrescriptionBuilder:BaseResourceInformationBuilder<MedicationPrescription>
    {
        protected override ResourceInfo GetRelatedPatientInformation(MedicationPrescription resource)
        {
          

            return new ResourceInfo() {ResourceId = resource.Patient?.Reference};
        }
    }
}
