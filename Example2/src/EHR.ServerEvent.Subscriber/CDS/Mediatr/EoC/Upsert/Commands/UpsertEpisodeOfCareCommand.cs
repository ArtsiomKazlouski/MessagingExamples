using System;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EoC.Upsert.Commands
{
    public class UpsertEpisodeOfCareCommand : IRequest
    {
        public UpsertEpisodeOfCareCommand(EpisodeOfCare episodeOfCare, string patientId)
        {
            if (episodeOfCare == null)
            {
                throw new ArgumentNullException(nameof(episodeOfCare));
            }
            if (string.IsNullOrWhiteSpace(patientId) == true)
            {
                throw new ArgumentNullException(nameof(patientId));
            }
            EpisodeOfCare = episodeOfCare;
            PatientId = patientId;
        }
        public EpisodeOfCare EpisodeOfCare { get; private set; }
        public string PatientId { get; private set; }
    }
}