using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticalCode.Extensions
{
    public class StatisticalCodeReference
    {
        public string ReasonTemporaryDisability { get; set; }
        public List<string> Icd10Codes { get; set; }

        public IEnumerable<Icd10Range> Icd10Ranges => Icd10Codes.Select(Icd10Range.Parce).ToList();

        public int StatisticalCode { get; set; }
        public List<StatisticalCodeReference> StatisticalCodesReferenceChild { get; set; }
    }

    public class Icd10Range
    {
        public Icd10Range(char @class, int subClassMin, int subClassMax)
        {
            Class = @class;
            SubClassMin = subClassMin;
            SubClassMax = subClassMax;
        }

        public char Class { get;  }

        public int SubClassMin { get;  }

        public int SubClassMax { get;  }


        public static Icd10Range Parce(string icd10Range)
        {
           var splited = icd10Range.Split(new[] {'-', '–'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

            var @class = splited.First()[0];

            return new Icd10Range(@class, int.Parse(splited.First().Substring(1)), int.Parse(splited.Last().Substring(1)));
        }
    }
}
