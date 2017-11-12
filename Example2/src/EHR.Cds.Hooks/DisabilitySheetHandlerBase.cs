using System;
using System.Linq;
using System.Threading.Tasks;
using EHR.Cds.Infrastructure.Util;
using EHR.Cds.Models;
using Hl7.Fhir.Model;

namespace EHR.Cds.Hooks
{
    public abstract class DisabilitySheetHandlerBase : IRecommendationHandler
    {

        private const string CompositionCreatingDateSystem =
            "http://fhir.org/fhir/StructureDefinition/by-disability-sheet-date-created";

        private const string EpisodeOfCareTreatmentModeSystem =
            "http://fhir.org/fhir/StructureDefinition/by-mode-treatment";

        private const string EncounterVkkProfile = "http://hl7.org/fhir/StructureDefinition/BY-SLC-Encounter-MAB";

        public async Task<Card> Handle(RequestMessage message)
        {
            var hookContext = message.Context as Bundle;
            if (hookContext == null)
                throw new ArgumentException("Для использования CDS в листках нетрудоспособности необходимо в свойстве Context передать FHIR ресурс типа Bundle");



            var episodeOfCare = GetResourceFromBundle<EpisodeOfCare>(hookContext);
            var composition = GetResourceFromBundle<Composition>(hookContext);
            var claim = GetResourceFromBundle<Claim>(hookContext);
            var condition = GetResourceFromBundle<Condition>(hookContext);
            var encounters = hookContext.Entry.Select(b=>b.Resource).OfType<Encounter>();

            
            var context = new DisabilitySheetHookContext
            {
               PatientId = message.Patient,
               TreatmentStart = FhirDateParser.Parse(episodeOfCare.Period.Start),
               TreatmentEnd = string.IsNullOrWhiteSpace(episodeOfCare.Period.End)
                                        ? FhirDateParser.Parse(episodeOfCare.Period.Start)
                                        : FhirDateParser.Parse(episodeOfCare.Period.End),
               CompositionCreating = FhirDateParser.Parse(composition.GetExtensionValue<Date>(CompositionCreatingDateSystem).Value),
               TreatmentModeCode =((CodeableConcept)episodeOfCare.Extension.First(e => e.Url.Equals(EpisodeOfCareTreatmentModeSystem)).Value).Coding.First().Code,
               DiagnosisCode = condition.Code.Coding.First().Code,
               KindDisabilityCode = claim.Condition.First().Code,
               VkkDates = encounters
                            .Where(e=> e.Meta?.Profile != null && e.Meta.Profile.Any(p=>p.Equals(EncounterVkkProfile)))
                            .Select(p=> FhirDateParser.Parse(p.Period.Start)),
               FreedPersonId = composition.Subject.Reference,
               EpisodeOfCareId = episodeOfCare.Id,
               ClaimId = claim.Id,
               CompositionId = composition.Id

            };

            return await HandleCore(context);
        }

        private T GetResourceFromBundle<T>(Bundle bundle)
        {
            var resource = bundle.Entry.Select(b=>b.Resource).OfType<T>().FirstOrDefault();
            if (resource == null)
            {
                throw new Exception($"Ресурс {typeof(T).Name} должен быть указан в списке ресурсов ресурса Bundle");
            }
            return resource;
        }



        protected abstract Task<Card> HandleCore(DisabilitySheetHookContext context);
    }
}