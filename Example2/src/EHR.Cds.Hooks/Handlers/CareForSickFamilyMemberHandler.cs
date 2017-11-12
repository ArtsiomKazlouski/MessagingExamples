using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using EHR.Cds.Models;
using Microsoft.Extensions.Options;

namespace EHR.Cds.Hooks.Handlers
{
  
    //todo переделать логику
    /// <summary>
    /// По уходу за больным членом семьи
    /// </summary>
    public class CareForSickFamilyMemberHandler: DisabilitySheetHandlerBase
    {
        private readonly CareForSickFamilyMemberOptions _options;

        /// <summary>
        /// Код вида нетрудоспособности по уходу за больным членом семьи
        /// </summary>
        private const string KindDisabilityCode = "careForSickFamilyMember";

        private readonly IUnitOfWork _unitOfWork;

        public CareForSickFamilyMemberHandler(IUnitOfWork unitOfWork, IOptions<DisabilitySheetCdsSettings> options)
        {
            _options = options.Value.CareForSickFamilyMemberOptions;
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Card> HandleCore(DisabilitySheetHookContext context)
        {
            //Если вид нетрудоспособности не уход за больным членом семьи то правило не проверяем
            if (string.Equals(context.KindDisabilityCode, KindDisabilityCode) ==false)
            {
                return null;
            }


            if ((context.TreatmentEnd - context.TreatmentStart).Days + 1 > 14)
            {
                return new Card("Больничный листок по уходу за больным членом семьи не должен превышать 14 дней", Indicator.HardStop, new Link(""));
            }

            var sql = @"
                        select 
                            (select EXTRACT(YEAR FROM age(cast(pat.date_birth as date))) from patient pat where pat.id = @PatientId ) as  age,
                            (
	                            SELECT
	                               SUM(DATE_PART('day', eoc.end::timestamp - eoc.start::timestamp)+1)   
	                            FROM composition com
	                            JOIN episodeofcare eoc on com.episodeofcare_id = eoc.id
	                            FULL OUTER JOIN patient pat on eoc.patient_id = pat.id
	                            where (
			                            (com.status != 'entered-in-error' and eoc.status!='cancelled' and com.claim_id = @ClaimId) 
			                            or com.claim_id is null or @ClaimId is null
		                            )  
	                            and (eoc.id!=@EpisodeOfCareId or eoc.id is null) 
	                            and pat.id = @PatientId
	                            Group by pat.id
                            ) as  dayscount
                            ;
                        ";
           
            var sqlParameters = new
            {
                context.ClaimId,
                context.EpisodeOfCareId, 
                context.PatientId
            };
            var fromDataBase = (await _unitOfWork.QueryAsync(
                (connection, transaction) => connection.QuerySingleOrDefaultAsync<dynamic>(sql, sqlParameters, transaction)));

            //Добавляем период Eoc Пришедший в cds
            var duration = (context.TreatmentEnd - context.TreatmentStart).Days + 1 + (fromDataBase == null ? 0 : fromDataBase.dayscount ?? 0);


            if (duration > 14)
            {
                return new Card("Больничный листок по уходу за больным членом семьи не должен превышать 14 дней", Indicator.HardStop, new Link(""));
            }

            if (fromDataBase == null || fromDataBase?.age == null)
            {
                
                if (duration <= 7)
                {
                    return new Card("Правило пройдено успешно", Indicator.Success, new Link(""));
                }
                if (duration <= 14)
                {
                    return new Card("Возраст пациента неизвестен. Больничный не может быть более 14 дней для детей до 14 лет, в остальных случаях 7 дней", Indicator.Warning, new Link(""));
                }

               
            }


            if (fromDataBase?.age > 14)
            {
                if (context.VkkDates.Any()==false)
                {
                    return new Card("Для Выписки больничного листка пациенту, чей возраст больше 14 лет, должно быть проведено ВКК", Indicator.HardStop, new Link(""));
                }
                if (duration > 7)
                {
                    return new Card("Больничный листок по уходу за больным членом семьи не должен превышать 7 дней", Indicator.HardStop, new Link(""));

                }
                
            }

            return new Card("Правило выполнено успешно", Indicator.Success, new Link(""));

        }






    }
}
