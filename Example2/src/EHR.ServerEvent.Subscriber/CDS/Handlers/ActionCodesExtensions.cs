using System.Linq;

namespace EHR.ServerEvent.Subscriber.CDS.Handlers
{
    public static class ActionCodesExtensions
    {
        private static readonly string[] UpsertActiuonCodes = {"created", "updated", "POST", "PUT"};
        private static readonly string[] DeleteActionCodes = {"deleted", "DELETE"};

        public static bool IsUpsertActionCode(this string actionCode)
        {
            return UpsertActiuonCodes.Contains(actionCode);
        }

        public static bool IsDeleteActionCode(this string actionCode)
        {
            return DeleteActionCodes.Contains(actionCode);
        }
    }
}
