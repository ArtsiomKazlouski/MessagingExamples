using EHR.FhirServer.Core;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Vread
{

    public class VreadRequestHandler: IRequestHandler<VreadRequest, Resource>
    {
        private readonly IFhirBase _fhirBase;
        public VreadRequestHandler(IFhirBase fhirBase)
        {
            _fhirBase = fhirBase;
        }

        public Resource Handle(VreadRequest message)
        {
            return _fhirBase.VRead(message.Type, message.Vid);
        }
    }
}