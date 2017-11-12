using Hl7.Fhir.Model;

namespace EHR.FhirServer.Core
{
    public interface IFhirBase
    {
        Resource Create(Resource resource);
        Resource Read(string type, string id);
        Resource VRead(string type, string vid);
        Resource Update(Resource resource);
        Resource Delete(string type, string id);
        Resource Search(string type, string query);
        Resource Conformance(string publisher, string version, string fhirVersion, bool acceptUnknown);
      
    }
}