using System;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.CDS.Mediatr.Composition.Delete.Command.Handlers
{
    public class DeleteCompositionCommandHandler : IAsyncRequestHandler<DeleteCompositionCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public DeleteCompositionCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(DeleteCompositionCommand message)
        {
            var statement = @"DELETE FROM composition
                                WHERE id=@Id;";
            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, new {Id = message.Id}, t));
            }
        }
        
    }
}