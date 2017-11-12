using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Conditions.Upsert.Handlers
{
    public class UpsertConditionCommandHandler : IAsyncRequestHandler<UpsertConditionCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public UpsertConditionCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(UpsertConditionCommand message)
        {
            var statement = @"INSERT INTO condition(
                                id, diagnosis_code, status, last_updated)
                                VALUES (@Id, @Code,@Status, @LastUpdated)
                              ON CONFLICT (id) DO UPDATE
                                SET diagnosis_code=EXCLUDED.diagnosis_code, last_updated=EXCLUDED.last_updated, status=EXCLUDED.status
                                WHERE condition.last_updated < EXCLUDED.last_updated;";

            var p = new
            {
                Status = message.Condition.ClinicalStatus.ToString(),
                Code = message.Condition.Code.Coding.First().Code,
                LastUpdated=  message.Condition.Meta.LastUpdated,
                Id= message.Condition.Id
            };
            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, p, t));
            }
        }
    }
}