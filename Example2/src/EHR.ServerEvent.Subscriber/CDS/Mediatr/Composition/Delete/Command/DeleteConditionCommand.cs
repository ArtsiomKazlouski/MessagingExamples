using System;
using MediatR;

namespace EHR.ServerEvent.Subscriber.CDS.Mediatr.Composition.Delete.Command
{
    public class DeleteCompositionCommand : IRequest
    {
        public DeleteCompositionCommand(string id)
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