using System;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Conditions.Upsert
{
    public class UpsertConditionCommand : IRequest
    {
        public UpsertConditionCommand(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            } 
            Condition = condition;
        }

        public Condition Condition { get; private set; }
    }
}