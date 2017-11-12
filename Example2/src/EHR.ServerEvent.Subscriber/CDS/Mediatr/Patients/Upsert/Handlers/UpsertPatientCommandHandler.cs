using System;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using EHR.Cds.Infrastructure.Util;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.Patients.Upsert.Handlers
{
    public class UpsertPatientCommandHandler : IAsyncRequestHandler<UpsertPatientCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public UpsertPatientCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(UpsertPatientCommand message)
        {
            var statement = @"INSERT INTO patient(
                                id, date_birth, last_updated)
                                VALUES (@Id, @DateBirth, @LastUpdated)
                              ON CONFLICT (id) DO UPDATE
                                SET date_birth=EXCLUDED.date_birth, last_updated=EXCLUDED.last_updated
                                WHERE patient.last_updated < EXCLUDED.last_updated;";

            var p = new
            {
                DateBirth = string.IsNullOrEmpty(message.Patient.BirthDate)?null:new DateTime?(FhirDateParser.Parse(message.Patient.BirthDate)),
                LastUpdated = message.Patient.Meta.LastUpdated,
                Id = message.Patient.Id
            };
            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, p, t));
            }
        }
        
    }
}