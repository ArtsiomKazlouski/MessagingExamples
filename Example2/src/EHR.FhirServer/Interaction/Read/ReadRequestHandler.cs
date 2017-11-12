using EHR.FhirServer.Core;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Read
{
    public class ReadRequestHandler: IRequestHandler<ReadRequest, Resource>
    {
        private readonly IFhirBase _fhirBase;

        public ReadRequestHandler(IFhirBase fhirBase)
        {
            _fhirBase = fhirBase;
        }

        public Resource Handle(ReadRequest message)
        {
            return _fhirBase.Read(message.Type, message.Id);
        }
    }
}