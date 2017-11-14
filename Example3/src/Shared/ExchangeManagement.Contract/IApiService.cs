using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeManagement.Contract.Messages;

namespace ExchangeManagement.Contract
{
    public interface IApiService
    {
        void SendResults(TaskCalculationResult result);
        TaskArguments Calculate(TaskArguments result);
        TaskCalculationResult GetResult(long taskId);
    }
}
