namespace EHR.FhirServer
{
    public class ApiData
    {
        public string Name { get; set; }
        public ApiDataScope[] Scopes { get; set; }
    }

    public class ApiDataScope
    {
        public string Name { get; set; }
    }
}
