using System;
using System.Globalization;

namespace EHR.Cds.Infrastructure.Util
{
    public static class FhirDateParser
    {
        public static DateTime Parse(string s)
        {
            return DateTimeOffset.Parse(s, CultureInfo.InvariantCulture).DateTime.Date;
        }
    }
}
