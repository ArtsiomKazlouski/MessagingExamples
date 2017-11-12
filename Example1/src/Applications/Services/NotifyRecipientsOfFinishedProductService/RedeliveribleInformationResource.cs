using System.Collections.Generic;
using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;

namespace NotifyRecipientsOfFinishedProductService
{ 
    public class RedeliveribleMessage
    {
        public MessageMetadata Message { get; set; }
        public IList<Subscription> Subscriptions { get; set; }
        public int DeliveryCount { get; set; }
    }
}
