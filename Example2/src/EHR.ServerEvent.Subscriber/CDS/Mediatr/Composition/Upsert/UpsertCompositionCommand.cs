using System;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.CDS.Mediatr.Composition.Upsert
{
    public class UpsertCompositionCommand : IRequest
    {
        public UpsertCompositionCommand(Hl7.Fhir.Model.Composition composition)
        {
            Composition = composition ?? throw new ArgumentNullException(nameof(composition));
        }

        public Hl7.Fhir.Model.Composition Composition { get; private set; }
    }
}