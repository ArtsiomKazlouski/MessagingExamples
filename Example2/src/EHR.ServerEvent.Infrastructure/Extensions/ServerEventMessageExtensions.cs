using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Infrastructure.Util;

namespace EHR.ServerEvent.Infrastructure.Extensions
{
    public static class ServerEventMessageExtensions
    {
        public static string GetRoutingGey(this ServerEventMessage message, string prefix)
        {
            return RoutingKeyHelper.GetRoutingGey(prefix, message.ResourceType, message.ActionCode);
        }
    }
}
