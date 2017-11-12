using System;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Patients.Delete.Command
{
    public class DeletePatientCommand : IRequest
    {
        public DeletePatientCommand(string id)
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