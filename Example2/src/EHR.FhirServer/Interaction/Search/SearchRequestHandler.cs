using System;
using System.Collections.Generic;
using System.Linq;
using EHR.FhirServer.Core;
using FluentValidation;
using FluentValidation.Results;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Search
{
    public class SearchRequestHandler : IRequestHandler<SearchRequest, Resource>
    {
        private readonly IFhirBase _fhirBase;
        private readonly Conformance _conformance;

        public SearchRequestHandler(IFhirBase fhirBase, Conformance conformance)
        {
            _fhirBase = fhirBase;
            _conformance = conformance;
        }

        public Resource Handle(SearchRequest message)
        {
            ValidateSearchparams(message.Type, message.Query);
            string query = string.Join("&", message.Query.Select(kv => string.Join("=", kv.Key, kv.Value)));
            return _fhirBase.Search(message.Type, query);
        }

        private void ValidateSearchparams(string type, IEnumerable<KeyValuePair<string, string>> searchParams)
        {
            string[] reserved =
            {
                "_id",
                "_lastUpdated",
                "_tag",
                "_profile",
                "_security",
                "_text",
                "_content",
                "_list",
                "_type",
                "_query",
                "_filter",

                "_sort",
                "_count",
                "_page",
                "_include",
                "_revinclude",
                "_summary",
                "_elements",
                "_contained",
                "_containedType",

                "_format"
            };

            var unsupportedSearchParams = searchParams
                .Select(kv => kv.Key)
                .Where(k => string.IsNullOrEmpty(k) == false)
                .Select(k => k
                    .Split(new[]
                    {
                        ":"
                    }, StringSplitOptions.RemoveEmptyEntries)
                    .First())
                .Except(reserved)
                .Except(_conformance.Rest.First()
                    .Resource
                    .Where(r => r != null)
                    .Where(r => string.IsNullOrEmpty(r.Type) == false)
                    .Where(r => r.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(r => r.SearchParam)
                    .Select(sp => sp.Name))
                .ToList();

            if (unsupportedSearchParams.Any())
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(null,
                        string.Format("Неизвестный параметр(ы) - '{0}'.", string.Join(",", unsupportedSearchParams)))
                });
            }
        }
    }
}