using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using EHR.Cds.Models;
using Microsoft.Extensions.Options;

namespace EHR.Cds.Hooks.Handlers
{


    //Превышение периода непрерывного срока нетрудоспособности
    public class ExcessTermContinuousPeriodDisabilityHandler : DisabilitySheetHandlerBase
    {
        private readonly ExcessTermContinuousPeriodDisabilityOptions _options;
        private readonly IUnitOfWork _unitOfWork;


        public ExcessTermContinuousPeriodDisabilityHandler(IUnitOfWork unitOfWork, IOptions<DisabilitySheetCdsSettings> options)
        {
            _options = options.Value.ExcessTermContinuousPeriodDisabilityOptions;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }


        protected override async Task<Card> HandleCore(DisabilitySheetHookContext context)
        {
            if (_options.AvailableKindDisabilityCodes.Any(
                    a => a.Equals(context.KindDisabilityCode, StringComparison.CurrentCultureIgnoreCase)) == false)
            {
                return null;
            }
            //Поскольку правило звучит как Превышение периода непрерывного срока нетрудоспособности, то нужно учитывать и предидущие и последующие листки нетрудоспособности
            var sql = $@"SELECT 
                            eoc.start as beginDate, 
                            eoc.end as endDate,
                            LOWER(diagnosis_code) SIMILAR TO '({string.Join("|", _options.TuberculosisMkbDiagnosis)})%' as IsTuberculosis
                        FROM episodeofcare eoc  
INNER JOIN episodeofcare_condition ecc ON ecc.episodeofcare_id = eoc.id 
                INNER JOIN condition ec ON ec.id = ecc.condition_id AND ec.status = 'final'
                    INNER JOIN composition com ON com.episodeofcare_id = eoc.id and com.status!='entered-in-error' 
                    WHERE eoc.status!='cancelled' and com.subject_id = @PatientId and eoc.id!=@EpisodeOfCareId";

            var existingDisabilitiesPeriods = (await _unitOfWork.QueryAsync(
                (connection, transaction) => connection.QueryAsync<ThreatmentRange>(sql, new { PatientId= context.FreedPersonId, context.EpisodeOfCareId }, transaction))).ToList();

            var isTuberculosisConditioninRequest =
                _options.TuberculosisMkbDiagnosis.Any(t => context.DiagnosisCode.ToLowerInvariant().StartsWith(t));
            existingDisabilitiesPeriods = existingDisabilitiesPeriods.Where(d => d.IsTuberculosis == isTuberculosisConditioninRequest).OrderBy(p => p.BeginDate).ToList();

            var continuousDisabilitiesPeriods = new List<ThreatmentRange>();


            DateTime? startDate = context.TreatmentStart;
            //Выбираем непрерывные переоды предшествующие
            while (startDate != null)
            {
               
                var previosPeriod =
                    existingDisabilitiesPeriods.FirstOrDefault(p => p.EndDate <= startDate.Value && (startDate.Value- p.EndDate).TotalHours <= 24);
                if (previosPeriod != null)
                {
                    continuousDisabilitiesPeriods.Add(previosPeriod);
                    startDate = previosPeriod.BeginDate;
                    continue;
                }
                startDate = null;
            }
            DateTime? endDate = context.TreatmentEnd;
            //Выбираем непрерывные переоды последующие
            while (endDate != null)
            {
                var nextPeriod =
                    existingDisabilitiesPeriods.FirstOrDefault(p => p.BeginDate >= endDate.Value && (p.BeginDate - endDate.Value).TotalHours <= 24);
                if (nextPeriod != null)
                {
                    continuousDisabilitiesPeriods.Add(nextPeriod);
                    endDate = nextPeriod.EndDate;
                    continue;
                }
                endDate = null;
            }
            continuousDisabilitiesPeriods.Add(new ThreatmentRange() { EndDate = context.TreatmentEnd, BeginDate = context.TreatmentStart, IsTuberculosis = _options.TuberculosisMkbDiagnosis.Any(d => d.Equals(context.DiagnosisCode, StringComparison.OrdinalIgnoreCase)) });

            //Выбираем из списка переодов те которые граничат с текущим с разницей в день
            //И по критерию туберкулезности должны соответствовать контексту
            //var continuousDisabilitiesPeriods = existingDisabilitiesPeriods.Where(
            //    p => (p.BeginDate - context.TreatmentStart).Hours <= TimeSpan.FromDays(1).Hours ||
            //         (p.EndDate - context.TreatmentEnd).Hours <= TimeSpan.FromDays(1).Hours).Where(d => d.IsTuberculosis == isTuberculosisConditioninRequest).ToList();



            var continiousDays = continuousDisabilitiesPeriods.Sum(p => (p.EndDate - p.BeginDate).Days+1);
            var maximumYearDays = isTuberculosisConditioninRequest ? _options.MaximumTuberculosisDaysCount : _options.MaximumDaysCountWithoutTuberculosis;

            if (continiousDays > maximumYearDays)
            {
                return new Card("Превышение периода непрерывного срока нетрудоспособности", Indicator.HardStop, new Link(""));
            }

            if (continiousDays >= maximumYearDays - _options.DaysPriorToExcees)
            {
                return new Card("Превышение периода непрерывного срока нетрудоспособности", Indicator.Warning, new Link(""));
            }

            return new Card("Правило пройдено успешно", Indicator.Success, new Link(""));

        }

        protected class ThreatmentRange
        {
            public DateTime BeginDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool IsTuberculosis { get; set; }
        }
    }
}
