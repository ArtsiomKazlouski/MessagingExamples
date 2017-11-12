using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;
using InfResourceManagement.Shared.Contracts.Types.InformationResource;

namespace NotifyRecipientsOfFinishedProductService
{
    public interface IImportService
    {
        long Import(Subscription subscription, MessageMetadata product);
    }
}
