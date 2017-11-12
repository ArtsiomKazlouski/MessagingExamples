using System;
using System.Collections.Generic;
using System.Linq;
using StatisticalCode.Extentions;

namespace StatisticalCode.Extensions
{
    public static class StatisticalCodeByICD10Extentions
    {
        public static int GetStatisticalCode(string diagnosisICD10)
        {
            var previousStatisticalCodeReferences = GetStatisticalCode(diagnosisICD10, Extensions.Extentions.Deserialize<List<StatisticalCodeReference>>(
                    "StatisticalCode.Extensions.StatisticalCodesReference.xml"));
            var statisticalCodeReferences = previousStatisticalCodeReferences;
            while (statisticalCodeReferences != null)
            {
                statisticalCodeReferences = GetStatisticalCode(diagnosisICD10, statisticalCodeReferences.StatisticalCodesReferenceChild);
                if (statisticalCodeReferences != null)
                {
                    previousStatisticalCodeReferences = statisticalCodeReferences;
                }
            }

            if (previousStatisticalCodeReferences == null)
                throw new ArgumentException("По указанному коду МКБ не найден статистический код", diagnosisICD10);
            return previousStatisticalCodeReferences.StatisticalCode;
        }

        private static StatisticalCodeReference GetStatisticalCode(string diagnosisICD10, IList<StatisticalCodeReference> codeReferences)
        {
            diagnosisICD10 = diagnosisICD10.Trim();
            char @class = diagnosisICD10[0];
            var subClass = int.Parse(diagnosisICD10.Substring(1, 2));



            return
               codeReferences
                    .FirstOrDefault(
                        c =>
                            c.Icd10Ranges.Any(
                                i => string.Equals(i.Class.ToString(), @class.ToString(),StringComparison.OrdinalIgnoreCase) && (subClass >= i.SubClassMin && subClass <= i.SubClassMax)));


        }

    }
}
