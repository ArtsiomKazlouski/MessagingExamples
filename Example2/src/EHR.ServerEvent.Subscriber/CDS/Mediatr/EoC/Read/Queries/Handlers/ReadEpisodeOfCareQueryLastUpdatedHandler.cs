using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EoC.Read.Queries.Handlers
{
    public class ReadEpisodeOfCareQueryLastUpdatedHandler: IAsyncRequestHandler<ReadEpisodeOfCareQueryLastUpdated, DateTimeOffset?>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public ReadEpisodeOfCareQueryLastUpdatedHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<DateTimeOffset?> Handle(ReadEpisodeOfCareQueryLastUpdated message)
        {
            var statement = @"SELECT last_updated
                              FROM episodeofcare
                              WHERE id = @Id";

            using (var uow = _unitOfWorkFactory())
            {
                return
                    (await uow.QueryAsync(
                            (c, t) => c.QueryAsync<DateTime?>(statement, new {Id = message.EpisodeOfCare.Id, t})))
                    .SingleOrDefault();
            }
        }
    }
}