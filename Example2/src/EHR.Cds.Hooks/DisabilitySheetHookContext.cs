using System;
using System.Collections.Generic;
using System.Text;
using EHR.Cds.Models;

namespace EHR.Cds.Hooks
{
    public class DisabilitySheetHookContext : HookContext
    {
        public string EpisodeOfCareId { get; set; }
        public string ClaimId { get; set; }
        public string CompositionId { get; set; }

        /// <summary>
        /// Начало периода нетрудоспособности
        /// </summary>
        public DateTime TreatmentStart { get; set; }

        /// <summary>
        /// Конец периода нетрудоспособности
        /// </summary>
        public DateTime TreatmentEnd { get; set; }


        /// <summary>
        /// Код диагноза
        /// </summary>
        public string DiagnosisCode { get; set; }


        /// <summary>
        /// Код вида наблюдения (стационарный амбулаторный)
        /// </summary>
        public string KindDisabilityCode { get; set; }


        /// <summary>
        /// Дата создания листка нетрудоспособности
        /// </summary>
        public DateTime CompositionCreating { get; set; }

        /// <summary>
        /// Дата проведения ВКК
        /// </summary>
        public IEnumerable<DateTime> VkkDates { get; set; }

        /// <summary>
        /// Код режима лечения
        /// </summary>
        public string TreatmentModeCode { get; set; }

        /// <summary>
        /// Освобожденный от труда
        /// </summary>
        public string FreedPersonId { get; set; }

    }

   
}
