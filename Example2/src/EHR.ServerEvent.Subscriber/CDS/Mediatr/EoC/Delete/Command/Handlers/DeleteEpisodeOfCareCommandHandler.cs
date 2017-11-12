using System;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EoC.Delete.Command.Handlers
{
    public class DeleteEpisodeOfCareCommandHandler : IAsyncRequestHandler<DeleteEpisodeOfCareCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public DeleteEpisodeOfCareCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(DeleteEpisodeOfCareCommand message)
        {
            var statement = @"DELETE FROM episodeofcare
                                WHERE id=@Id;";
            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, new {Id = message.Id}, t));
            }
        }
    }
}