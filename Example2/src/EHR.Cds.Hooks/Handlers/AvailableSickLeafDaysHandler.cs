using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using EHR.Cds.Models;
using Microsoft.Extensions.Options;

namespace EHR.Cds.Hooks.Handlers
{



    //Превышение общего периода нетрудоспособности за последние 12 месяцев:
    public class AvailableSickLeafDaysHandler : DisabilitySheetHandlerBase
    {
        private readonly AvailableSickLeafDaysOptions _options;


        private readonly IUnitOfWork _unitOfWork;


        public AvailableSickLeafDaysHandler(IUnitOfWork unitOfWork, IOptions<DisabilitySheetCdsSettings> options)
        {
            _options = options.Value.AvailableSickLeafDaysOptions;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override async Task<Card> HandleCore(DisabilitySheetHookContext context)
        {
            if (_options.AvailableKindDisabilityCodes.Any(
                a => a.Equals(context.KindDisabilityCode, StringComparison.CurrentCultureIgnoreCase))==false)
            {
                return new Card("Правило для указанного вида нетрудоспособности не проверяеться", Indicator.Info, new Link(""));
            }

            var isTuberculosisConditioninRequest =
                _options.TuberculosisMkbDiagnosis.Any(t => context.DiagnosisCode.ToLowerInvariant().StartsWith(t));



            var requestingDays = (context.TreatmentEnd.Date - context.TreatmentStart.Date).Days+1;


            var calculateDaysSql =
                "CASE WHEN start< (current_date - interval '1 year') THEN \"end\"::date-(current_date - interval '1 year')::date+1  ELSE \"end\"::date-start::date+1 END";


            var sqlGetDays =
                $@"SELECT SUM(CASE WHEN LOWER(diagnosis_code) SIMILAR TO '({string.Join("|", _options.TuberculosisMkbDiagnosis)})%'  
                   THEN {(isTuberculosisConditioninRequest ? calculateDaysSql : 0.ToString())} 
                 ELSE {(isTuberculosisConditioninRequest == false ? calculateDaysSql : 0.ToString())} END) AS days 
                FROM episodeofcare eoc 
                INNER JOIN episodeofcare_condition ecc ON ecc.episodeofcare_id = eoc.id 
                INNER JOIN condition ec ON ec.id = ecc.condition_id AND ec.status = 'Confirmed'
                    INNER JOIN composition com ON com.episodeofcare_id = eoc.id  and  com.status != 'entered-in-error' " +
                "WHERE \"end\">(current_date - interval '1 year') and eoc.status!='cancelled' and com.subject_id = @PatientId and eoc.id!=@EpisodeOfCareId";


            var queryParam = new
            {
                PatientId= context.FreedPersonId,
                context.EpisodeOfCareId,
            };

            var query = (
                await _unitOfWork.QueryAsync(
                    (connection, transaction) =>
                        connection.QueryAsync<long?>(sqlGetDays, queryParam, transaction))).First();
            var queryResult = query ?? 0;
            var usedDaysInLastYear = queryResult;
            var maximumYearDays = isTuberculosisConditioninRequest
                ? _options.MaximumTuberculosisDaysCount
                : _options.MaximumDaysCountWithoutTuberculosis;


            if (usedDaysInLastYear + requestingDays > maximumYearDays)
            {
                return new Card("Превышение общего периода нетрудоспособности за последние 12 месяцев.", Indicator.HardStop, new Link(""));
            }

            if (usedDaysInLastYear + requestingDays >= maximumYearDays - _options.DaysPriorToExcees)
            {
                return new Card("Превышение общего периода нетрудоспособности за последние 12 месяцев.", Indicator.Warning, new Link(""));
            }

            return new Card("Правило пройдено успешно", Indicator.Success, new Link(""));
        }


    }
}
