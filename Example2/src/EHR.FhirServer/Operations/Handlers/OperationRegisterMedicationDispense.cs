using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EHR.FhirServer.Core;
using EHR.FhirServer.Exceptions;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;

namespace EHR.FhirServer.Operations.Handlers
{
    public class OperationRegisterMedicationDispense : BaseMedicalPrescriptionOperation
    {
        private const string MedicationDispenseParameterName = "medicationDispense";
        private const string ClaimParameterName = "claim";
      

        protected override string OperationName => OperationConstants.RegisterMedicationDispenseOPerationName;

        protected override bool ExecuteOnSpecificInstance => false;

        protected override IEnumerable<ResourceType> SupportedTypes => new List<ResourceType>() { ResourceType.MedicationDispense };


    
        public OperationRegisterMedicationDispense(IFhirBase fhirBase, IHttpContextAccessor httpContextAccessor) : base(fhirBase, httpContextAccessor)
        {
           
        }

        internal class OperationParameters
        {
            

            public OperationParameters(Parameters parameters)
            {

                var parametersDictonary = new Dictionary<string, Action<Parameters.ParametersParameterComponent>>()
                {
                    {MedicationDispenseParameterName,(e) => MedicationDispense = e.Resource as MedicationDispense },
                    {ClaimParameterName,(e) => Claim = e.Resource as Claim },
                 
                };


                foreach (var parameter in parameters.Parameter)
                {
                    Action<Parameters.ParametersParameterComponent> action;
                    if (parametersDictonary.TryGetValue(parameter.Name, out action))
                    {
                        action.Invoke(parameter);
                    }
                }
              
            }

            public MedicationDispense MedicationDispense { get; private set; }
            public Claim Claim { get; private set; }
        }

        protected override Resource ExecuteCore(EnrichedOperationContext context)
        {
            var resultBundle = new Bundle() { Type = Bundle.BundleType.Collection, Entry = new List<Bundle.BundleEntryComponent>() };
            if (context.OperationContext.OperationParameters.Parameter.Count(p =>
                    p.Name.Equals(MedicationDispenseParameterName)) != 1)
            {
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Необходимо указать один MedicationDispense");
            }


            var parameters = new OperationParameters(context.OperationContext.OperationParameters);

            if (parameters.MedicationDispense == null)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Необходимо указать MedicationDispense");
            if (parameters.MedicationDispense.AuthorizingPrescription== null || parameters.MedicationDispense.AuthorizingPrescription.Count != 1)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Необходимо в MedicationDispense указать ссылку на один MedicationPrescription");
           if (parameters.MedicationDispense.Status != MedicationDispense.MedicationDispenseStatus.Completed)
               throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "У MedicationDispense статус должен быть Completed");
            if (parameters.MedicationDispense.Contained == null ||
                parameters.MedicationDispense.Contained.Count(e => e.ResourceType == ResourceType.Location) != 1)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Необходимо в MedicationDispense указать один ресурс Location");

           

            var medicationPrescription = GetAndValidateMedicationPrescription(parameters.MedicationDispense);
            if (medicationPrescription.Status != MedicationPrescription.MedicationPrescriptionStatus.Active)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "У MedicationPrescription статус должен быть Active");

            GetAndValidatePatient(medicationPrescription);
        
            var medicationPrescriptionFactor = medicationPrescription.Extension.FirstOrDefault(e => e.Url.Equals(MedicalPrescriptionFactorUrl))?.Value as FhirDecimal;

           
            if (medicationPrescriptionFactor?.Value != null && medicationPrescriptionFactor.Value >= 0 &&
                medicationPrescriptionFactor.Value < 1)
            {

                if (context.OperationContext.OperationParameters.Parameter.Count(p =>
                        p.Name.Equals(ClaimParameterName)) != 1)
                {
                    throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Для льготных рецептов должен указываться один ресурс Claim");
                }

                if (parameters.Claim.Item == null || parameters.Claim.Item.Count > 1 || parameters.Claim.Item.Any() == false)
                {
                    throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Для льготных рецептов должен указываться один ресурс в секции Товары и услуги (Claim.Item)");
                }
                var claimItem = parameters.Claim.Item.First();
                if (claimItem.Factor == null)
                {
                    throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Необходимо указать Коэфициент для определения суммы оплаты");
                }

                if (claimItem.Factor == null)
                {
                    throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Необходимо указать Коэфициент для определения суммы оплаты");
                }
                if (claimItem.Factor + medicationPrescriptionFactor?.Value != 1)
                {
                    throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Процент оплаты организации не соответствует информации из электронного рецепта");
                }
            }
        
                
                
              
               
           
            //Рецепт не льготный Claim быть не должно
            if ((medicationPrescriptionFactor?.Value == null || medicationPrescriptionFactor.Value == 1) && parameters.Claim != null)
                throw new FhirHttpResponseException(HttpStatusCode.BadRequest, "Для рецептов, не являющихся льготными, ресурс Claim не нужен");


            //MedicationDispense embedded resource Location  установить ссылку на организацию (аптечную сеть), полученную с помощью ОКПО
            (parameters.MedicationDispense.Contained.FirstOrDefault(e => e.ResourceType == ResourceType.Location) as Location).ManagingOrganization = context.OrganizationReference;




            var createdMedicationDispense = FhirBase.Create(parameters.MedicationDispense).GetResult<MedicationDispense>();
            resultBundle.Entry.Add(new Bundle.BundleEntryComponent(){Resource = createdMedicationDispense});

            if (parameters.Claim != null)
            {  
                //Claim установить ссылку на организацию (аптечную сеть), полученную с помощью ОКПО
                parameters.Claim.Payee = new Claim.PayeeComponent(){Organization =context.OrganizationReference };
                parameters.Claim.SetExtension(ClaimMedicalDispanceUrl,
                    new ResourceReference()
                    {
                        Reference = $"{createdMedicationDispense.TypeName}/{createdMedicationDispense.Id}"
                    });
                var createdClaim = FhirBase.Create(parameters.Claim).GetResult<Claim>();
                resultBundle.Entry.Add(new Bundle.BundleEntryComponent() { Resource = createdClaim });


            }

            var medicationDispenseFinal = parameters.MedicationDispense.Extension.FirstOrDefault(e => e.Url.Equals(MedicationDispanceFinalDispanceUrl))?.Value as FhirBoolean;
            if (parameters.MedicationDispense.Status == MedicationDispense.MedicationDispenseStatus.Completed &&
                medicationDispenseFinal != null && medicationDispenseFinal.Value == true)
            {
                medicationPrescription.Status = MedicationPrescription.MedicationPrescriptionStatus.Completed;
                 FhirBase.Update(medicationPrescription).GetResult<MedicationPrescription>();
             
            }


            return resultBundle;

        }
    }
}
