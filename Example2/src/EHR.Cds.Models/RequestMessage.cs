using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hl7.Fhir.Model;


namespace EHR.Cds.Models
{
    [DataContract]
    public class RequestMessage
    {
        [DataMember(Name = "hook")]
        public Link Hook { get; set; }

        [DataMember(Name = "hookInstance")]
        public Guid HookInstance { get; set; }

        [DataMember(Name = "fhirServer")]
        public Uri FhirServer { get; set; }

        [DataMember(Name = "oauth")]
        public object Oauth { get; set; }


        [DataMember(Name = "redirect")]
        public Uri Redirect { get; set; }

        [DataMember(Name = "user")]
        public string User { get; set; }

        [DataMember(Name = "patient")]
        public string Patient { get; set; }

        [DataMember(Name = "encounter")]
        public string Encounter { get; set; }

        [DataMember(Name = "prefetch")]
        public Dictionary<string, object> Prefetch { get; set; }

        [DataMember(Name = "context")]
        public Resource Context { get; set; }



        public RequestMessage()
        {
            
        }

        public RequestMessage(RequestMessage message)
        {
            Hook = message.Hook;
            HookInstance = message.HookInstance;
            FhirServer = message.FhirServer;

            Context = message.Context;
            Redirect = message.Redirect;
            User = message.User;
            Patient = message.Patient;
            Encounter = message.Encounter;
            Prefetch = message.Prefetch;
        }
    }

   


}
