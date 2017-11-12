using System;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EoC.Delete.Command
{
    public class DeleteEpisodeOfCareCommand : IRequest
    {
        public DeleteEpisodeOfCareCommand(string id)
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