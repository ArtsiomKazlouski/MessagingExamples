using System.Collections.Generic;
using System.Linq;
using System.Net;
using EHR.FhirServer.Core;
using EHR.FhirServer.Exceptions;
using EHR.FhirServer.Extensions;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;

namespace EHR.FhirServer.Operations.Handlers
{
    public class OperationSetAsErrorMedicationDispense : BaseMedicalPrescriptionOperation
    {
        protected override string OperationName => OperationConstants.SetAsErrorMedicationDispenseOPerationName;

        protected override bool ExecuteOnSpecificInstance => true;

        protected override IEnumerable<ResourceType> SupportedTypes => new List<ResourceType>() { ResourceType.MedicationDispense };

        

        public OperationSetAsErrorMedicationDispense(IFhirBase fhirBase, IHttpContextAccessor httpContextAccessor) : base(fhirBase, httpContextAccessor)
        {
            
        }


        protected override Resource ExecuteCore(EnrichedOperationContext context)
        {
            var medicalDispanceIdentifier = context.OperationContext.ResourceId;

            var medicalDispance = FhirBase.Read(ResourceType.MedicationDispense.ToString(), medicalDispanceIdentifier) as MedicationDispense;
            if (medicalDispance == null)
                throw new FhirHttpResponseException(HttpStatusCode.NotFound, "MedicationDispense не найден");

            if (medicalDispance.Status != MedicationDispense.MedicationDispenseStatus.Completed && medicalDispance.Status != MedicationDispense.MedicationDispenseStatus.InProgress)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "У MedicationDispense должен быть статус complete или in-progress");

        
            var medicationPrescription = GetAndValidateMedicationPrescription(medicalDispance);
            if (medicationPrescription.Status != MedicationPrescription.MedicationPrescriptionStatus.Active && medicationPrescription.Status != MedicationPrescription.MedicationPrescriptionStatus.Completed)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "У MedicationPrescription статус должен быть Active или Completed");


            var medicalDispanceLocation =
                medicalDispance.Contained?.FirstOrDefault(e => e.ResourceType == ResourceType.Location) as Location;
            if (medicalDispanceLocation==null)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "У в MedicationDispense не указан один ресурс Location");

            if (medicalDispanceLocation.ManagingOrganization.ExtractId() != context.OrganizationId)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Аптека может отменять только MedicationDispense, которые были созданы только её аптечной сетью.");


            GetAndValidatePatient(medicationPrescription);



            //Обновляеться статус для MedicationDispense
            medicalDispance.Status = MedicationDispense.MedicationDispenseStatus.EnteredInError;
            FhirBase.Update(medicalDispance).GetResult<MedicationDispense>();
         

            //Удаляеться Claim
            var ballClaims = GetAllClaimsForPrescriptions(new[] {medicationPrescription});
            var claim = ballClaims.FirstOrDefault(e=>e.GetExtensionValue<ResourceReference>(ClaimMedicalDispanceUrl)?.ExtractId() == medicalDispance.Id);
            if (claim != null)
            {
                FhirBase.Delete(ResourceType.Claim.ToString(), claim.Id);
              
            }

            //Устанавливаеться MedicationPrescription статус Active
            if (medicationPrescription.Status != MedicationPrescription.MedicationPrescriptionStatus.Active)
            {
                medicationPrescription.Status = MedicationPrescription.MedicationPrescriptionStatus.Active;
                FhirBase.Update(medicationPrescription).GetResult<MedicationPrescription>();
            }
          




            // Устанавливаеться  MedicationDispense.finalDispence = false для всех MedicationDispense
            var alledicationDispance = GetAllMedicationDIspanceForPrescriptions(new []{ medicationPrescription });
            foreach (var medicationDispance in alledicationDispance.Where(m=>m.Id!=context.OperationContext.ResourceId)
                .Where(m => m.GetExtensionValue<FhirBoolean>(MedicationDispanceFinalDispanceUrl)?
                                .Value == true))
            {
                medicationDispance.SetExtension(MedicationDispanceFinalDispanceUrl,
                    new FhirBoolean(false));
                FhirBase.Update(medicationDispance).GetResult<MedicationDispense>();
               
            }
           
            return new OperationOutcome() { Issue = { new OperationOutcome.OperationOutcomeIssueComponent() { Details = "Операция успешно выполнена", Severity = OperationOutcome.IssueSeverity.Information, Code = new CodeableConcept("http://hl7.org/fhir/http-code", HttpStatusCode.OK.ToString()) } } };

        }
    }
}
