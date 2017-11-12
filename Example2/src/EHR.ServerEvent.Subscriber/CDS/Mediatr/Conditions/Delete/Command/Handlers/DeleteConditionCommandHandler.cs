using System;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Conditions.Delete.Command.Handlers
{
    public class DeleteConditionCommandHandler : IAsyncRequestHandler<DeleteConditionCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public DeleteConditionCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(DeleteConditionCommand message)
        {
            string statement = @"DELETE FROM condition
                                WHERE id=@Id;";
            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, new {Id = message.Id}, t));
            }
        }
        
    }
}