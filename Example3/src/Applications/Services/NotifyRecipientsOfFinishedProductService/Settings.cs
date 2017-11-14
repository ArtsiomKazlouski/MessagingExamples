using System.Configuration;

namespace WorkerService
{
    public class Settings
    {
        public string FinishedProductExchanger { get; } = ConfigurationManager.AppSettings["finishedProductExchanger"];
        public string FinishedProductQueue { get; } = ConfigurationManager.AppSettings["finishedProductQueue"];
        public string ApiEndpoint { get; } = ConfigurationManager.AppSettings["ApiEndpoint"];
        public string ServiceName { get; } = "NotifyRecipientsOfFinishedProductService";
    }
}
