using System;
using System.Collections.Generic;
using System.Text;
using EHR.ServerEvent.Infrastructure;
using EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceInformationReader
{
    public class GetAction : ResourceInformationReader
    {
        public GetAction(ResourceBuilderFuctory buildersFuctory, IEnumerable<PayloadReader.PayloadReader> payloadReaders) 
            : base(buildersFuctory, payloadReaders, Microsoft.AspNetCore.Http.HttpMethods.Get)
        {

        }
    }

    public class PutAction : ResourceInformationReader
    {
        public PutAction(ResourceBuilderFuctory buildersFuctory,
            IEnumerable<PayloadReader.PayloadReader> payloadReaders)
            : base(buildersFuctory, payloadReaders, Microsoft.AspNetCore.Http.HttpMethods.Put)
        {

        }

        public override IEnumerable<ServerEventResourceInfromation> ResourceInformation(ActionMetadata actionMetadata)
        {
            var resource = base.ReadPayloadFromResponce.Invoke(actionMetadata);

            //Если в результате выполнгения произошла ошибка и вернулся OperationOutcome то попытаемся взять информацию из реквеста
            if (resource is OperationOutcome)
            {
                resource = base.ReadPayloadFromRequest.Invoke(actionMetadata);
            }
            return BuildersFuctory.GetBuilder(resource).Build(resource, actionMetadata);
        }
    }

    public class PathAction : ResourceInformationReader
    {
        public PathAction(ResourceBuilderFuctory buildersFuctory, IEnumerable<PayloadReader.PayloadReader> payloadReaders)
            : base(buildersFuctory, payloadReaders, Microsoft.AspNetCore.Http.HttpMethods.Patch)
        {

        }
    }

    public class PostAction : ResourceInformationReader
    {
        public PostAction(ResourceBuilderFuctory buildersFuctory, IEnumerable<PayloadReader.PayloadReader> payloadReaders)
            : base(buildersFuctory, payloadReaders, Microsoft.AspNetCore.Http.HttpMethods.Post)
        {

        }
    }


    public class DeleteAction : ResourceInformationReader
    {
        public DeleteAction(ResourceBuilderFuctory buildersFuctory, IEnumerable<PayloadReader.PayloadReader> payloadReaders)
            : base(buildersFuctory, payloadReaders, Microsoft.AspNetCore.Http.HttpMethods.Delete)
        {

        }
    }
}
