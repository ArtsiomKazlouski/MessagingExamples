using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHR.Cds.Models
{
    /// <summary>
    /// service that handle specific context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICdsService
    {
        Service DiscoveryIdentifier { get; }

        Task<IEnumerable<Card>> HandleAsync(RequestMessage requestMessage);
    }
}
