using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;

namespace WorkerService
{
    public interface IApiService
    {
        void SendResults(TaskCalculationResult result);
    }
}
