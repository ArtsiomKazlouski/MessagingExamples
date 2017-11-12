using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHR.Cds.Models
{
    /// <summary>
    /// handler
    /// </summary>
    public interface IRecommendationHandler
    {
        Task<Card> Handle(RequestMessage message);
    }
}