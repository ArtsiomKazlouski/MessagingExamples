using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatisticalCode.Extensions;

namespace StatisticalCode.Extentions
{
    public static class StatisticalCode
    {
        public static int GetStatisticalCode(string diagnosisICD10Code, string kindDisabilityCode)
        {
            
            var statisticalCodeByKindDisability = StatisticalCodeByKindDisability.GetStatisticalCode(kindDisabilityCode);
            if (statisticalCodeByKindDisability != null)
                return statisticalCodeByKindDisability.Value;


            return StatisticalCodeByICD10Extentions.GetStatisticalCode(diagnosisICD10Code);
        }
    }
}
