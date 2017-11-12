using System;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Patients.Delete.Command.Handlers
{
    public class DeletePatientCommandHandler : IAsyncRequestHandler<DeletePatientCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public DeletePatientCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(DeletePatientCommand message)
        {
            var statement = @"DELETE FROM patient
                                WHERE id=@Id;";
            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, new {message.Id}, t));
            }
        }
    }
}