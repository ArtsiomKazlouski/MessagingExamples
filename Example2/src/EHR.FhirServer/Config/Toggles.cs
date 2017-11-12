
namespace EHR.FhirServer.Config
{
    public class Toggles
    {
        public bool Authentication { get; set; } = true;
        public bool ServerEvent { get; set; } = true;
        public bool Nginx { get; set; } = false;
    }
}
