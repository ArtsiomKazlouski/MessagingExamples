using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.CDS.Mediatr.Composition.Read.Queries.Handlers
{
    public class ReadCompositionLastUpdatedQueryHandler : IAsyncRequestHandler<ReadCompositionLastUpdatedQuery, DateTimeOffset?>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public ReadCompositionLastUpdatedQueryHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<DateTimeOffset?> Handle(ReadCompositionLastUpdatedQuery message)
        {
            var statement = @"SELECT last_updated
                              FROM composition
                              WHERE id = @Id";
            using (var uow = _unitOfWorkFactory())
            {
                return (await uow.QueryAsync(
                            (c, t) => c.QueryAsync<DateTime?>(statement, new {Id = message.Id, t})))
                    .SingleOrDefault();
            }
        }
    }
}