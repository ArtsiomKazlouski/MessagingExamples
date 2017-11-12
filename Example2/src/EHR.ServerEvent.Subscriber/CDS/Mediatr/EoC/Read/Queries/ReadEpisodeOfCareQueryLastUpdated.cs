using System;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EoC.Read.Queries
{
    public class ReadEpisodeOfCareQueryLastUpdated: IRequest<DateTimeOffset?>
    {
        public ReadEpisodeOfCareQueryLastUpdated(EpisodeOfCare episodeOfCare)
        {
            if (episodeOfCare == null)
            {
                throw new ArgumentNullException(nameof(episodeOfCare));
            }
            EpisodeOfCare = episodeOfCare;
        }
        public EpisodeOfCare EpisodeOfCare { get; private set; }
    }
}