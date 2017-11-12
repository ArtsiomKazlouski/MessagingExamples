using System;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Conditions.Delete.Command
{
    public class DeleteConditionCommand : IRequest
    {
        public DeleteConditionCommand(string id)
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