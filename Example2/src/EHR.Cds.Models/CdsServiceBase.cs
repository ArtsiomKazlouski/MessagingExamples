using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHR.Cds.Models
{
    public abstract class CdsServiceBase: ICdsService{

        protected IEnumerable<IRecommendationHandler> Handlers { get; }

        public abstract  Task<IEnumerable<Card>> HandleAsync(RequestMessage requestMessage);

        protected CdsServiceBase(IEnumerable<IRecommendationHandler> handlers)
        {
            Handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        }

        public abstract Service DiscoveryIdentifier { get; }
    }
}