using System;
using System.Linq;
using System.Threading.Tasks;
using EHR.Cds.Models;
using Microsoft.Extensions.Options;

namespace EHR.Cds.Hooks.Handlers
{



    //Дата выписки листка нетрудоспособности не должна превышать дату начала нетрудоспособности (дату освобождения от работы -
    //TreatmentStart берется из EpisodeOfCare.Period.Start в случае продления ЛН дата начала нетурдоспособности (Claim.extension.Period.Start) будет меньше даты выписки)
    public class CompositionStatementDateHandler : DisabilitySheetHandlerBase
    {
        private readonly CompositionStatementDateOptions _options;

        /// <summary>
        /// Код режима наблюдения  - стационарный
        /// </summary>
        private const string TreatmentModeInpatient = "inpatient";

        /// <summary>
        /// Код режима наблюдения - амбулаторный
        /// </summary>
        private const string TreatmentModeMmbulatory = "ambulatory";

        public CompositionStatementDateHandler(IOptions<DisabilitySheetCdsSettings> options)
        {
            _options = options.Value.CompositionStatementDateOptions;
        }


        protected override async Task<Card> HandleCore(DisabilitySheetHookContext context)
        {


            if (context.CompositionCreating <= context.TreatmentStart)
                return new Card("Правило пройдено успешно", Indicator.Success, new Link(""));

            if (string.Equals(context.TreatmentModeCode, TreatmentModeInpatient))
            {
                return new Card("Дата выписки листка нетрудоспособности не должна превышать дату освобождения от работы при выполнении условия -  режим должен быть стационарный.", Indicator.Success, new Link(""));
            }

            if (context.VkkDates != null)
            {
                if (string.Equals(context.TreatmentModeCode, TreatmentModeMmbulatory) && context.VkkDates.Any(d => d.Date == context.CompositionCreating.Date))
                {
                    return new Card("Дата выписки листка нетрудоспособности не должна превышать дату освобождения от работы при выполнении условия -  режим амбулаторный и проведено ВКК в день выписки.", Indicator.Success, new Link(""));
                }
            }


            return new Card("Дата выписки листка нетрудоспособности не должна превышать дату освобождения от работы", Indicator.HardStop, new Link(""));

        }
    }
}
