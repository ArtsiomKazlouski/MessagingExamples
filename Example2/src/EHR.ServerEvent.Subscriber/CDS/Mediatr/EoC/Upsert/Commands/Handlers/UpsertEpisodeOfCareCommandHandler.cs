using System;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using EHR.Cds.Infrastructure.Util;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Mediatr.EoC.Upsert.Commands.Handlers
{
    public class UpsertEpisodeOfCareCommandHandler : IAsyncRequestHandler<UpsertEpisodeOfCareCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFcatory;

        public UpsertEpisodeOfCareCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFcatory = unitOfWorkFactory;
        }

        public async Task Handle(UpsertEpisodeOfCareCommand message)
        {
            var statement = @"INSERT INTO episodeofcare("+
                               " id, patient_id, start, \"end\", last_updated, status)"+
                              @"VALUES (@Id, @PatientId, @Start, @End, @LastUpdated, @Status)
                              ON CONFLICT (id) DO UPDATE
                              SET patient_id=EXCLUDED.patient_id, start=EXCLUDED.start," +
                               " \"end\"=EXCLUDED.end, last_updated=EXCLUDED.last_updated, status=EXCLUDED.status" +
                            "  WHERE episodeofcare.last_updated < EXCLUDED.last_updated;";
            var p = new
            {
                Id = message.EpisodeOfCare.Id,
                PatientId = message.PatientId,
                Start = FhirDateParser.Parse(message.EpisodeOfCare.Period.Start),
                End = string.IsNullOrWhiteSpace(message.EpisodeOfCare.Period.End)
                    ? FhirDateParser.Parse(message.EpisodeOfCare.Period.Start)
                    : FhirDateParser.Parse(message.EpisodeOfCare.Period.End),
                LastUpdated = message.EpisodeOfCare.Meta.LastUpdated,
                Status = message.EpisodeOfCare.Status
            };
            using (var uow = _unitOfWorkFcatory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, p, t));
            }
        }
    }
}