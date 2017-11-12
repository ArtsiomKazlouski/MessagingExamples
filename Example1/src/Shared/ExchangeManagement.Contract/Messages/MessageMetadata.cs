using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeManagement.Contract.Messages
{
    public class MessageMetadata
    {
        public string Content { get; set; }
    }

    public abstract class ExchangerNames
    {
        public const string InformationResource = "InformationResource";
        public const string DemoPicture = "DemoPicture";
        public const string QuickLook = "QuickLook";
        public const string DownloadResourceFile = "ResourceFile";
    }

    public abstract class MessageTopics
    {
        public const string Created = "created";
        public const string Updated = "updated";
        public const string Deleted = "deleted";
    }

    public interface IFinishedProductService
    {
        bool IsReady(MessageMetadata message);
    }

    public class StubFinishedProductService: IFinishedProductService
    {
        public bool IsReady(MessageMetadata message)
        {
            if (message?.Content == null)
            {
                return false;
            }

            return message.Content.StartsWith("r", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
