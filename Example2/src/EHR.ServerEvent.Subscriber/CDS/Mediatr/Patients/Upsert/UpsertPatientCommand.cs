using System;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Patients.Upsert
{
    public class UpsertPatientCommand : IRequest
    {
        public UpsertPatientCommand(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            Patient = patient;
        }

        public Patient Patient { get; private set; }
    }
}