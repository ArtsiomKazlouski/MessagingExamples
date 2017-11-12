using System;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EocCondition.Delete.Command.Handlers
{
    public class DeleteEocConditionCommandHandler : IAsyncRequestHandler<DeleteEocConditionCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public DeleteEocConditionCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(DeleteEocConditionCommand message)
        {
            var statement = @"DELETE FROM episodeofcare_condition
                                WHERE episodeofcare_id=@EpisodeOfCareId AND condition_id=@ConditionId;";
            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, new {message.EpisodeOfCareId, message.ConditionId}, t));
            }
        }
    }
}