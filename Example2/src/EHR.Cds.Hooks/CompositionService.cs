using System.Collections.Generic;
using System.Threading.Tasks;
using EHR.Cds.Models;

namespace EHR.Cds.Hooks
{
    public class DisabilitySheetService : CdsServiceBase
    {
        public DisabilitySheetService(IEnumerable<IRecommendationHandler> handlers) : base(handlers)
        {
        }

        public override async Task<IEnumerable<Card>> HandleAsync(RequestMessage requestMessage)
        {
            var resultsCards = new List<Card>();
            foreach (var recommendationHandler in  this.Handlers)
            {
                var result = await recommendationHandler.Handle(requestMessage);
                if (result != null)
                {
                    resultsCards.Add(result);
                }
               
            }
            return resultsCards;
        }

        public override Service DiscoveryIdentifier { get; } = new Service("DisabilitySheet",
            "DisabilitySheet", "DisabilitySheet CDS");
    }
}