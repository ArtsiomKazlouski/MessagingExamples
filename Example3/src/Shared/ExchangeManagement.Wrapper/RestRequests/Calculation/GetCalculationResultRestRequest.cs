using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Wrapper.RestRequests.Subscription;

namespace ExchangeManagement.Wrapper.RestRequests.Calculation
{
   

    public class GetCalculationResultRestRequest : BaseCalcualtionRestRequest
    {
        public GetCalculationResultRestRequest(long taskId)
            : base(taskId.ToString(),HttpMethod.Get)
        {
            
        }
    }
}
