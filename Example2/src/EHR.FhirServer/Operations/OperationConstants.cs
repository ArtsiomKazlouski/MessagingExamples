using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHR.FhirServer.Operations
{
    public static class OperationConstants
    {

        public const string GetMedicationPrescriptionByIdCardOPerationName = "by-card";
        public const string RegisterMedicationDispenseOPerationName = "register";
        public const string SetAsErrorMedicationDispenseOPerationName = "set-as-error";
    }
}
