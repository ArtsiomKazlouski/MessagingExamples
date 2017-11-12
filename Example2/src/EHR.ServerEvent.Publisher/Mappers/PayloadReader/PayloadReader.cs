using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.PayloadReader
{
    /// <summary>
    /// Стратегия получения ресурса из массива хранящегосяв бд на основе content-type из Headers
    /// </summary>
    public abstract class PayloadReader
    {
        protected PayloadReader(string[] contentType)
        {
            ContentTypes = contentType;
        }

        protected string[] ContentTypes { get; }

        public virtual bool CanRead(IEnumerable<KeyValuePair<string, string>> headers)
        {
            var contentType = GetContentTypePart(headers.FirstOrDefault(h => h.Key.Equals("Content-Type", StringComparison.CurrentCultureIgnoreCase)).Value);
            return ContentTypes.Any(h => string.Equals(h, contentType, StringComparison.CurrentCultureIgnoreCase));
        }

        public abstract Resource Read(string base64String);

        private string GetContentTypePart(string contentType)
        {
            return string.IsNullOrWhiteSpace(contentType)
                ? contentType
                : contentType.Split(';')[0].Trim();
        }
    }
}
