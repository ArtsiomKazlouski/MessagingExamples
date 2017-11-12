using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;

namespace NotifyRecipientsOfFinishedProductService
{
    public interface IImportService
    {
        long Import(Subscription subscription, MessageMetadata product);
    }
}
