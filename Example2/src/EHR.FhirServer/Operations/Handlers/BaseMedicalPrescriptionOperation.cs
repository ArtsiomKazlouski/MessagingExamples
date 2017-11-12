using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EHR.FhirServer.Core;
using EHR.FhirServer.Exceptions;
using EHR.FhirServer.Extensions;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;

namespace EHR.FhirServer.Operations.Handlers
{
    public abstract class BaseMedicalPrescriptionOperation:OperationHandlerBase
    {
        protected class EnrichedOperationContext 
        {
            public EnrichedOperationContext(OperationContext context, string organizationId)
            {
                OperationContext = context;
                OrganizationId = organizationId;
            }

            public OperationContext OperationContext { get;  }

            public string OrganizationId { get;  }

            public ResourceReference OrganizationReference => new ResourceReference(){Reference = $"{ResourceType.Organization}/{OrganizationId}"};
        }

        public static string OrganizationIdClaimName = "client_organization_id";

        protected IFhirBase FhirBase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected const string MedicalPrescriptionFactorUrl = "http://fhir.org/fhir/StructureDefinition/by-factor";
        protected const string ClaimMedicalDispanceUrl = "http://fhir.org/fhir/StructureDefinition/by-dispense";
        protected const string MedicationDispanceFinalDispanceUrl = "http://fhir.org/fhir/StructureDefinition/by-finalDispense";

        protected BaseMedicalPrescriptionOperation(IFhirBase fhirBase, IHttpContextAccessor httpContextAccessor)
        {
            FhirBase = fhirBase;
            _httpContextAccessor = httpContextAccessor;
        }

        public override Resource Execute(OperationContext context)
        {

           //Получение ОКПО организации 
           var organizationIdClaim =  _httpContextAccessor.HttpContext.User.FindFirst(OrganizationIdClaimName);
            if (organizationIdClaim == null)
            {
                throw new FhirHttpResponseException(HttpStatusCode.Forbidden, "Не удалось получить идентификатор организации");
            }

        

          
            return ExecuteCore(new EnrichedOperationContext(context, organizationIdClaim.Value));
        }


        protected abstract Resource ExecuteCore(EnrichedOperationContext context);


        protected MedicationPrescription GetAndValidateMedicationPrescription(MedicationDispense medicationDispense)
        {
            var medicationPrescription = FhirBase.Read(ResourceType.MedicationPrescription.ToString(), medicationDispense.AuthorizingPrescription.First().ExtractId()) as MedicationPrescription;

            if (medicationPrescription == null)
                throw new FhirHttpResponseException(HttpStatusCode.NotFound, "MedicationPrescription на который ссылаеться MedicationDispense не найден");
          
            if (DateTime.Parse(medicationPrescription.Dispense.ValidityPeriod.End) < DateTime.Today)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "У MedicationPrescription не активный срок действия");

            return medicationPrescription;
        }


        protected Patient GetAndValidatePatient(MedicationPrescription medicationPrescription)
        {
            var patient = FhirBase.Read(ResourceType.Patient.ToString(), medicationPrescription.Patient.ExtractId()) as Patient;
            if (patient == null)
                throw new FhirHttpResponseException(HttpStatusCode.NotFound, "Пациент, на которого ссылается MedicationPrescription, не найден");
            if (patient.Active != true)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Пациент, на которого ссылается MedicationPrescription, не активный");

            return patient;
        }



        protected IEnumerable<MedicationDispense> GetAllMedicationDIspanceForPrescriptions(IEnumerable<MedicationPrescription> medicalPrescriptions)
        {
            string prescriptionsIds = string.Join(",", medicalPrescriptions.Select(m => m.Id));


            return FhirBase.Search(ResourceType.MedicationDispense.ToString(), $"prescription={prescriptionsIds}&status=completed,in-progress&_count={ int.MaxValue}")
                .GetResult<Bundle>().Entry.Select(e => e.Resource).OfType<MedicationDispense>();
        }


        protected IEnumerable<Claim> GetAllClaimsForPrescriptions(IEnumerable<MedicationPrescription> medicalPrescriptions)
        {
            string prescriptionsIds = string.Join(",", medicalPrescriptions.Select(m => m.Id));

            return FhirBase.Search(ResourceType.Claim.ToString(), $"originalPrescription={prescriptionsIds}&_count={ int.MaxValue}").GetResult<Bundle>().Entry.Select(e => e.Resource).OfType<Claim>();
        }
    }
}
