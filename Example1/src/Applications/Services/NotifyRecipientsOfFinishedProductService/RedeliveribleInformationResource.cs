using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;
using InfResourceManagement.Shared.Contracts.Types.InformationResource;

namespace NotifyRecipientsOfFinishedProductService
{ 
    public class RedeliveribleMessage
    {
        public MessageMetadata Message { get; set; }
        public IList<Subscription> Subscriptions { get; set; }
        public int DeliveryCount { get; set; }
    }
}
