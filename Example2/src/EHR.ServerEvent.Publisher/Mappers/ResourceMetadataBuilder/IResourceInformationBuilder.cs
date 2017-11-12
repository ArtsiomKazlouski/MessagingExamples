using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHR.ServerEvent.Infrastructure;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace EHR.ServerEvent.Publisher.Mappers.ResourceMetadataBuilder
{
    public class ResourceBuilderFuctory
    {
        private readonly IServiceProvider _builders;
        public ResourceBuilderFuctory(IServiceProvider builders)
        {
            _builders = builders;
        }

        public IResourceInformationBuilder GetBuilder(Resource resource)
        {
            return _builders.GetServices(typeof(IResourceInformationBuilder)).OfType<IResourceInformationBuilder>().FirstOrDefault(b => b.CanBuild(resource.GetType())) ?? new ResourceInformationBuilder();
        }
    }

    /// <summary>
    /// Стратегия получения ServerEventResourceInfromation из ресурса
    /// Стратегия получения ServerEventResourceInfromation из ресурса
    /// </summary>
    public interface IResourceInformationBuilder
    {
        bool CanBuild(Type resourceType);
        IEnumerable<ServerEventResourceInfromation> Build(Resource resource, ActionMetadata actionMetadata);
    }


  
    public abstract class BaseResourceInformationBuilder<T> : IResourceInformationBuilder where T : Resource
    {


        public virtual bool CanBuild(Type resourceType)
        {
            return typeof(T).Name == resourceType.Name;
        }

        public IEnumerable<ServerEventResourceInfromation> Build(Resource resource, ActionMetadata actionMetadata)
        {
            return BuildCore((T)resource, actionMetadata);
        }


        protected virtual IEnumerable<ServerEventResourceInfromation> BuildCore(T resource, ActionMetadata actionMetadata)
        {
            var resourceinfo = GetResource(resource);
            var patientinfo = GetRelatedPatientInformation(resource);
            var encounterinfo = GetRelatedEncounter(resource);
            var scope = GetScope(resource);
            var resourceJsonString = FhirSerializer.SerializeResourceToJson(resource);
            yield return new ServerEventResourceInfromation()
            {
                ResourceType = resource.TypeName,
                ResourceId = resourceinfo.ResourceId,
                ResourceVersion = resourceinfo.ResourceVersionId,
                EncounterId = encounterinfo.ResourceId,
                EncounterVersionId = encounterinfo.ResourceVersionId,
                PatientId = patientinfo.ResourceId,
                PatientVersion = patientinfo.ResourceVersionId,
                Scope = scope,
                ResourceJson = resourceJsonString
            };
        }


        protected virtual ResourceInfo GetResource(Resource resource)
        {
            return new ResourceInfo() { ResourceId = resource.Id, ResourceVersionId = resource.VersionId };
        }



        protected virtual string GetScope(T resource)
        {
            if (resource.Meta == null)
                return null;
            return string.Join("; ", resource.Meta?.Profile);
        }


        protected virtual ResourceInfo GetRelatedPatientInformation(T resource)
        {
            return new ResourceInfo();
        }

        protected virtual ResourceInfo GetRelatedEncounter(T resource)
        {
            return new ResourceInfo();
        }


        protected class ResourceInfo
        {
            public string ResourceId { get; set; }
            public string ResourceVersionId { get; set; }
        }
    }
}
