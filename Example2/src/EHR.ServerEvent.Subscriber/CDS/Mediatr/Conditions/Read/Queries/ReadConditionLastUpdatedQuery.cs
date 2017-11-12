using System;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Conditions.Read.Queries
{
    public class ReadConditionLastUpdatedQuery : IRequest<DateTimeOffset?>
    {
        public ReadConditionLastUpdatedQuery(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            Id = condition.Id;
        }
        public ReadConditionLastUpdatedQuery(string id)
        {
            if (string.IsNullOrWhiteSpace(id) == true)
            {
                throw new ArgumentNullException(nameof(id));
            }
            Id = id;
        }
        public string Id { get; private set; }
    }
}