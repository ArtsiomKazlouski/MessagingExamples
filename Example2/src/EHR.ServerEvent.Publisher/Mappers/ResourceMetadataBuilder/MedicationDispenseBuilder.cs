using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
   
    public class MedicationDispenseBuilder : BaseResourceInformationBuilder<MedicationDispense>
    {
        protected override ResourceInfo GetRelatedPatientInformation(MedicationDispense resource)
        {


            return new ResourceInfo() { ResourceId = resource.Patient?.Reference };
        }
    }
}
