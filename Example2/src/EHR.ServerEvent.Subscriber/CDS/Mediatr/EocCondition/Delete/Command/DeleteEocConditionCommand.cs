using System;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EocCondition.Delete.Command
{
    public class DeleteEocConditionCommand : IRequest
    {
        public DeleteEocConditionCommand(string episodeOfCareId, string conditionId)
        {
            if (string.IsNullOrWhiteSpace(episodeOfCareId) == true)
            {
                throw new ArgumentNullException(nameof(episodeOfCareId));
            }

            if (string.IsNullOrWhiteSpace(conditionId) == true)
            {
                throw new ArgumentNullException(nameof(ConditionId));
            }


            EpisodeOfCareId = episodeOfCareId;
            ConditionId = conditionId;

        }
        public string EpisodeOfCareId { get; private set; }
        public string ConditionId { get; private set; }

    }
}