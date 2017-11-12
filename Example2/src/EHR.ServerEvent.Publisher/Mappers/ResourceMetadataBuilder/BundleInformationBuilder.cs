using System.Collections.Generic;
using EHR.ServerEvent.Infrastructure;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class BundleInformationBuilder : BaseResourceInformationBuilder<Bundle>
    {
        protected readonly ResourceBuilderFuctory BuildersFuctory;

        public BundleInformationBuilder(ResourceBuilderFuctory buildersFactory)
        {
            BuildersFuctory = buildersFactory;
        }


        protected override IEnumerable<ServerEventResourceInfromation> BuildCore(Bundle resource, ActionMetadata actionMetadata)
        {
            foreach (var bundleEntryComponent in resource.Entry)
            {
                var builder = BuildersFuctory.GetBuilder(bundleEntryComponent.Resource);

                var result = builder.Build(bundleEntryComponent.Resource, actionMetadata);
                foreach (var serverEventResourceInfromation in result)
                {
                    yield return serverEventResourceInfromation;
                }



            }

        }
    }
}
