using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.CDS.Mediatr.Composition.Upsert.Handlers
{
    public class UpsertCompositionCommandHandler : IAsyncRequestHandler<UpsertCompositionCommand>
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;
        private static string claimCode = "disabilityState";
        private static string eocCode = "episodeOfCare";

        public UpsertCompositionCommandHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task Handle(UpsertCompositionCommand message)
        {
            var statement = @"INSERT INTO composition(
                                id, claim_id, episodeofcare_Id,subject_id, last_updated, status)
                                VALUES (@Id, @ClaimId,@EpisodeOfCareId, @SubjectId, @LastUpdated, @Status)
                              ON CONFLICT (id) DO UPDATE
                                SET claim_id=EXCLUDED.claim_id, episodeofcare_Id=EXCLUDED.episodeofcare_Id, subject_id=EXCLUDED.subject_id, status=EXCLUDED.status
                                WHERE composition.last_updated < EXCLUDED.last_updated;";
            
            var claimId = GetContainedId(message.Composition, "Claim", claimCode);
            var eocId = GetContainedId(message.Composition, "EpisodeOfCare", eocCode);
            var p = new
            {
                Id = message.Composition.Id,
                ClaimId = claimId,
                EpisodeOfCareId = eocId,
                SubjectId = message.Composition.Subject?.Reference,
                LastUpdated =  message.Composition.Meta.LastUpdated,
                Status = message.Composition.Status,
            };

            using (var uow = _unitOfWorkFactory())
            {
                await uow.QueryAsync((c, t) => c.ExecuteAsync(statement, p, t));
            }
        }

        private static string GetContainedId(Hl7.Fhir.Model.Composition composition, string resourceType, string sectionCode)
        {
            var resourceId = string.Empty;
            var containedId = composition.Section
                                     .FirstOrDefault(c => c.Code.Coding.Any(coding => coding.Code.Equals(sectionCode)))?.Content
                                     .Reference?.TrimStart('#') ?? string.Empty;
            if (string.IsNullOrWhiteSpace(containedId))
            {
                return resourceId;
            }
            
            var containedList = composition.Contained.FirstOrDefault(c => containedId.Equals(c.Id) && c.ResourceType == ResourceType.List) as List;
            if (containedList != null)
            {
                resourceId = containedList.Entry.FirstOrDefault()?.Item?.Reference
                                 ?.Replace($"{resourceType}/", string.Empty) ?? string.Empty;
            }

            return resourceId;
        }
    }
}