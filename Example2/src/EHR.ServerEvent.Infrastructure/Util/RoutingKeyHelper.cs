namespace EHR.ServerEvent.Infrastructure.Util
{
    public static class RoutingKeyHelper
    {
        public static string GetRoutingGey(string prefix, string resourceType, string actionCode)
        {
            return $"{prefix.Trim('.')}.{resourceType}.{actionCode}";
        }
    }
}
