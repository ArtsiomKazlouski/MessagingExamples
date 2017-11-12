using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StatisticalCode.Extensions
{
    public static class StatisticalCodeByKindDisability
    {
        public static int? GetStatisticalCode(string kindDisability)
        {
            var result =
               Extentions.Deserialize<List<KindOfDisabilityStatisticalCodeReference>>(
                       "StatisticalCode.Extensions.KindOfDisabilityStatisticalCodeReference.xml")
                    .FirstOrDefault(
                        c =>
                           string.Equals(c.KindDisabilityId.Trim(),kindDisability.Trim(), StringComparison.OrdinalIgnoreCase));
            if (result != null)
            {
                return result.StatisticalCode;
            }
            else
            {
                throw new ArgumentException("Не удалось найти указанный вид нетрудоспособности в справочнике", kindDisability);
            }
        }
    }
}
