using ExchangeManagement.Contract;
using InfResourceManagement.Shared.Contracts.Types.InformationResource;

namespace NotifyRecipientsOfFinishedProductService
{
    public interface IImportService
    {
        long Import(Subscription subscription, AggregateInformationResourceDetails product);
    }
}
