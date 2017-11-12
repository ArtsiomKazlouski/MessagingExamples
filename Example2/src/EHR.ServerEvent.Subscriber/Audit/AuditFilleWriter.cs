using System;
using System.IO;
using System.Threading.Tasks;
using EHR.ServerEvent.Subscriber.Contract;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EHR.ServerEvent.Subscriber.Audit
{
    public class AuditFilleWriter : IWriter<AuditEventMessage>
    {
        private readonly string _dataBaseFilePath;

        public AuditFilleWriter(IOptions<FileEventConfiguration> options)
        {
          
            if(Directory.Exists(Path.GetDirectoryName(options.Value.DataBasePath))==false)
            {
                throw new DirectoryNotFoundException("Указанная директория не найдена");
            }
            if (string.IsNullOrEmpty(Path.GetFileName(options.Value.DataBasePath)))
            {
                throw new ArgumentException("Необходимо указать название файла");
            }
            _dataBaseFilePath = options.Value.DataBasePath;
        }


        public async Task WriteAsync(AuditEventMessage eventMessage)
        {
            
            using (var writer = File.AppendText(_dataBaseFilePath))
            {
              await  writer.WriteAsync(JsonConvert.SerializeObject(eventMessage)); 
            }
            
           
        }
    }


    public class FileEventConfiguration
    {
        public FileEventConfiguration()
        {
            DataBasePath = Path.GetFullPath(@"EventDataBase.json");
        }

        public string DataBasePath { get; set; }
    }
}