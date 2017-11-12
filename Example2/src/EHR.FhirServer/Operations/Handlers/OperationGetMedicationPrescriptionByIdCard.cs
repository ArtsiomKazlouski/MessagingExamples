using System;
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
    public class OperationGetMedicationPrescriptionByIdCard: BaseMedicalPrescriptionOperation
    {

        public static string PatientIdentifierParameterName = "patientIdentifier";
        public static string MedicalPrescriptionStatusParameterName = "status";
        public static string MedicalCardIdentifierSystem = "http://uiip.bas-net.by/hl7/fhir/patient-medication-card";

        protected override string OperationName => OperationConstants.GetMedicationPrescriptionByIdCardOPerationName;

        protected override bool ExecuteOnSpecificInstance => false;

        protected override IEnumerable<ResourceType> SupportedTypes =>new List<ResourceType>(){ResourceType.MedicationPrescription};

     

        public OperationGetMedicationPrescriptionByIdCard(IFhirBase fhirBase, IHttpContextAccessor httpContextAccessor):base(fhirBase, httpContextAccessor)
        {
           
        }



        protected override Resource ExecuteCore(EnrichedOperationContext context)
        {
            var patientIdentifier = context.OperationContext.OperationParameters?.Get(PatientIdentifierParameterName).FirstOrDefault()?.Value as FhirString;
            var medicationPrescriptionStatus = context.OperationContext.OperationParameters?.Get(MedicalPrescriptionStatusParameterName).FirstOrDefault()?.Value as FhirString;

            if (patientIdentifier == null)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest,
                    "Необходимо указать параметр patientIdentifier");

            var patient = FhirBase
                .Search(ResourceType.Patient.ToString(), $"identifier={patientIdentifier.Value}")
                .GetResult<Bundle>().Entry.Select(e => e.Resource).OfType<Patient>().FirstOrDefault();
            if (patient == null || patient.Active ==false)
            {
                throw new FhirHttpResponseException(HttpStatusCode.NotFound, "Пациент с указанным идентификатором не найден");

            }


            //Получаються все MedicationPrescriptions без учета даты
            //Потомучто при создании SearchParameter with xpatx f:MedicationPrescription/f:dispense/f:validityPeriod когда в MedicationPrescriptions указан не валидный переод
            //получаеться ошибка - "22000: range lower bound must be less than or equal to range upper bound"

            //При попытки создания SearchParameter with xpatx f:MedicationPrescription/f:dispense/f:validityPeriod/f:end
            //запрос select * from fhir.search_sql('MedicationPrescription', 'end=>2010-06-17')
            //SELECT ... WHERE((fhirbase_date_idx.index_as_date(medicationprescription.content, '{dispense,validityPeriod,end}'::text[], NULL::text) && '["2010-06-17 00:00:00+03",)'))
            //а должно быть SELECT ... WHERE((fhirbase_date_idx.index_as_date(medicationprescription.content, '{dispense,validityPeriod,end}'::text[], 'date') && '["2010-06-17 00:00:00+03",)'))
            //нечто похожее уже обсуждали https://groups.google.com/forum/#!topic/fhirbase/fiuPApM7P9g
            //Поэтому отсеивание по датам происходит дальше в коде после получения из бд
            var medicationPrescriptions = FhirBase.Search(ResourceType.MedicationPrescription.ToString(), 
                $"patient:Patient.identifier={patientIdentifier.Value}" +
                $"&status={ (string.IsNullOrEmpty(medicationPrescriptionStatus?.Value)? "active": medicationPrescriptionStatus.Value)}" +
                $"&_count={int.MaxValue}")
                .GetResult<Bundle>().Entry.Select(e => e.Resource).OfType<MedicationPrescription>().Where(m =>
                    {
                      
                        DateTime.TryParse(m.Dispense?.ValidityPeriod?.End, out var parsedEndDate);
                      return parsedEndDate >= DateTime.Today;
                }
                ).ToList();
          



            if (medicationPrescriptions.Any() == false)
                return new Bundle(){Type = Bundle.BundleType.Searchset};
          


            var allMedicationDispense = GetAllMedicationDIspanceForPrescriptions(medicationPrescriptions).ToList();

            var allClaims = GetAllClaimsForPrescriptions(medicationPrescriptions).ToList();
          

          
            var result = new Bundle(){Entry = new List<Bundle.BundleEntryComponent>()};
            result.Entry.Add(new Bundle.BundleEntryComponent(){Resource = new Patient(){Name = patient.Name, Active = patient.Active, BirthDate = patient.BirthDate, Gender = patient.Gender, Identifier = patient.Identifier.Where(i=>i.System.Equals(MedicalCardIdentifierSystem)).ToList()}});
            foreach (var prescription in medicationPrescriptions)
            {

                var entry = new List<Bundle.BundleEntryComponent>()
                {
                    new Bundle.BundleEntryComponent()
                    {
                        Search = new Bundle.BundleEntrySearchComponent() {Mode = Bundle.SearchEntryMode.Match},
                        Resource = prescription
                    }
                };
                entry.AddRange(allMedicationDispense.Where(m => m.AuthorizingPrescription.Any(p => p.ExtractId() == prescription.Id)).Select(m=>new Bundle.BundleEntryComponent(){Resource = m, Search = new Bundle.BundleEntrySearchComponent(){Mode = Bundle.SearchEntryMode.Include}} ));

                entry.AddRange(allClaims.Where(m => m.OriginalPrescription.ExtractId() == prescription.Id).Select(m => new Bundle.BundleEntryComponent() { Resource = m, Search = new Bundle.BundleEntrySearchComponent() { Mode = Bundle.SearchEntryMode.Include } }));




                result.Entry.Add(new Bundle.BundleEntryComponent(){Resource = new Bundle(){Entry = entry}});
            }

            return result;


        }



       
    }
}
