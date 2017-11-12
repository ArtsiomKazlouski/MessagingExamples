using System;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.CDS.Mediatr.Composition.Read.Queries
{
    public class ReadCompositionLastUpdatedQuery : IRequest<DateTimeOffset?>
    {
        public ReadCompositionLastUpdatedQuery(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            Id = condition.Id;
        }
        public ReadCompositionLastUpdatedQuery(string id)
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