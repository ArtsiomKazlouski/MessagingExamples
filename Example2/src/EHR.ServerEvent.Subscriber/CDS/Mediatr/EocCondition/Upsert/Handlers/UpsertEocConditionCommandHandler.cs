using System;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EocCondition.Upsert.Handlers
{
    public class UpsertEocConditionCommandHandler : IAsyncRequestHandler<UpsertEocConditionCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public UpsertEocConditionCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(UpsertEocConditionCommand message)
        {
            var statement = @"INSERT INTO episodeofcare_condition(episodeofcare_id, condition_id)
                                          select @EpisodeOfCareId,  @ConditionId
              where not exists (select from episodeofcare_condition 
						                where episodeofcare_id = @EpisodeOfCareId
						                and condition_id =  @ConditionId)";

            var p = new
            {
                EpisodeOfCareId = message.EpisodeOfCare.Id,
                ConditionId = message.ConditionId,
            
            };
            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, p, t));
            }
        }
    }
}