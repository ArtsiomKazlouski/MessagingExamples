using InfResourceManagement.Shared.Contracts.Types;
using InfResourceManagement.Shared.Contracts.Types.Demopictures;

namespace NotifyRecipientsOfFinishedProductService
{
    public interface ITimeDemoPictureService
    {
        PagedResult<DemoPicture> Search(string query);
    }
}
