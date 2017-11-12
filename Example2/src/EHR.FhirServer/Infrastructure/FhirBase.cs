using System.Data;
using EHR.FhirServer.Core;
using EHR.FhirServer.Exceptions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Npgsql;
using NpgsqlTypes;

namespace EHR.FhirServer.Infrastructure
{
    public class FhirBase : IFhirBase

    {
        private readonly IFhirDataBaseProvider _dataBaseProvider;

        public FhirBase(IFhirDataBaseProvider dataBaseProvider)
        {
            _dataBaseProvider = dataBaseProvider;
        }

        public Resource Create(Resource resource)
        {
            return ExecuteFunction("fhir.create", new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Jsonb,
                Value = FhirSerializer.SerializeResourceToJson(resource)
            });
        }

        public Resource Read(string type, string id)
        {
            return ExecuteFunction("fhir.read", new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Text,
                Value = type
            },
                new NpgsqlParameter
                {
                    NpgsqlDbType = NpgsqlDbType.Text,
                    Value = id
                });
        }

        public Resource VRead(string type, string vid)
        {
            return ExecuteFunction("fhir.vread", new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Text,
                Value = type
            },
                new NpgsqlParameter
                {
                    NpgsqlDbType = NpgsqlDbType.Text,
                    Value = vid
                });
        }

        public Resource Update(Resource resource)
        {
            return ExecuteFunction("fhir.update", new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Jsonb,
                Value = FhirSerializer.SerializeResourceToJson(resource)
            });
        }

        public Resource Delete(string type, string id)
        {
            return ExecuteFunction("fhir.delete", new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Text,
                Value = type
            },
                new NpgsqlParameter
                {
                    NpgsqlDbType = NpgsqlDbType.Text,
                    Value = id
                });
        }

        public Resource Search(string type, string query)
        {
            return ExecuteFunction("fhir.search", new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Text,
                Value = type
            },
                new NpgsqlParameter
                {
                    NpgsqlDbType = NpgsqlDbType.Text,
                    Value = query
                });
        }

        public Resource Conformance(string publisher, string version, string fhirVersion, bool acceptUnknown)
        {
            var conformance = new Conformance
            {
                Publisher = publisher,
                Version = version,
                FhirVersion = fhirVersion,
                AcceptUnknown = acceptUnknown,
                Format = new []{"xml", "json"},
                Id = "FHIR Server",
                Software = new Conformance.ConformanceSoftwareComponent()
                {
                    Version = version
                },
                Date = "2015-05-01"
            };
            return ExecuteFunction("fhir.conformance", new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Jsonb,
                Value = FhirSerializer.SerializeResourceToJson(conformance)
            });
        }

       


        private Resource BuildResource(string resourceStr)
        {
            Resource resource = (Resource) FhirParser.ParseFromJson(resourceStr);

            OperationOutcome operationOutcome = resource as OperationOutcome;
            if (operationOutcome != null)
            {
                throw new FhirHttpResponseException(operationOutcome);
            }

            return resource;
        }

        private Resource ExecuteFunction(string name, params NpgsqlParameter[] parameters)
        {
            var resource = _dataBaseProvider.WithConnection(name, parameters);
           
            OperationOutcome operationOutcome = resource as OperationOutcome;
            if (operationOutcome != null)
            {
                throw new FhirHttpResponseException(operationOutcome);
            }

            return resource;

           
               
        }
    }
}