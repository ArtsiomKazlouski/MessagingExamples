using System;

namespace EHR.Cds.Models
{
    public abstract class HookContext
    {
        /// <summary>
        /// Идентификатор пациента на которого выписываеться листок нетрудоспособности
        /// </summary>
        public string PatientId { get; set; }

    }

   
}
