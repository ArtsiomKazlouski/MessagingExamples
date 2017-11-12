using ExchangeManagement.Contract.Messages;

namespace NotifyRecipientsOfFinishedProductService
{
    public interface ICheckSubscriptionService
    {
        bool Check(MessageMetadata message, string query);
    }
}
