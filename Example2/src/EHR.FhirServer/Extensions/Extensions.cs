using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;

namespace EHR.FhirServer.Extensions
{
    public static class Extensions
    {
        public static Bundle Flatten(this Bundle bundleToFlattern)
        {
            List<Bundle.BundleEntryComponent> entryComponents = new List<Bundle.BundleEntryComponent>();

            var itemsToProcessing = bundleToFlattern.Entry.Select(t => t.Resource).ToList();

            while (itemsToProcessing.Any())
            {
                var entry = itemsToProcessing.First();
                itemsToProcessing.Remove(entry);
                var bundle = entry as Bundle;
                if (bundle != null)
                {
                    itemsToProcessing.AddRange(bundle.Entry.Select(t => t.Resource));
                }
                else
                {
                    entryComponents.Add(new Bundle.BundleEntryComponent() {Resource = entry});
                }
            }

            return new Bundle() {Entry = entryComponents};
        }

        public static string ExtractId(this ResourceReference refference)
        {
            return refference.Reference.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }
    }
}
