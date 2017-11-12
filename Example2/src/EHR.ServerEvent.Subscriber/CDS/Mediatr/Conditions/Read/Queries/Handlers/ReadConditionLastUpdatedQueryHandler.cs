using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Conditions.Read.Queries.Handlers
{
    public class ReadConditionLastUpdatedQueryHandler : IAsyncRequestHandler<ReadConditionLastUpdatedQuery, DateTimeOffset?>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public ReadConditionLastUpdatedQueryHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<DateTimeOffset?> Handle(ReadConditionLastUpdatedQuery message)
        {
            var statement = @"SELECT last_updated
                              FROM condition
                              WHERE id = @Id";
            using (var uow = _unitOfWorkFactory())
            {
                return (await uow.QueryAsync(
                    (c, t) => c.QueryAsync<DateTime?>(statement, new {Id = message.Id, t}))).SingleOrDefault();

            }
        }
    }
}