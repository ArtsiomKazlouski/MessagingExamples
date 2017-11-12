using System;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EocCondition.Upsert
{
    public class UpsertEocConditionCommand : IRequest
    {
        public UpsertEocConditionCommand(EpisodeOfCare episodeOfCare, Condition condition)
            : this(episodeOfCare, condition.Id)
        {
        }

        public UpsertEocConditionCommand(EpisodeOfCare episodeOfCare, string conditionId)
        {
            if (episodeOfCare == null)
            {
                throw new ArgumentNullException(nameof(episodeOfCare));
            }
            if (conditionId == null)
            {
                throw new ArgumentNullException(nameof(conditionId));
            }
         
            EpisodeOfCare = episodeOfCare;
            ConditionId = conditionId;
           
        }

        public EpisodeOfCare EpisodeOfCare { get; private set; }
        public string ConditionId { get; private set; }
       
    }
}