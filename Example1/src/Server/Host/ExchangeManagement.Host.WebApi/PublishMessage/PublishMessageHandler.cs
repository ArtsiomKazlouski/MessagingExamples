using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExchangeManagement.Contract.Messages;
using MediatR;

namespace ExchangeManagement.Host.WebApi.PublishMessage
{
    public class PublishMessageRequest : IAsyncRequest
    {
        public PublishMessageRequest(MessageMetadata messageMetadata)
        {
            MessageMetadata = messageMetadata;
        }

        /// <summary>
        /// Сообщение для отправки
        /// </summary>
        public MessageMetadata MessageMetadata { get; set; }
    }

    public class PublishMessageHandler:AsyncRequestHandler<PublishMessageRequest>
    {
        protected override Task HandleCore(PublishMessageRequest message)
        {
            throw new NotImplementedException("publish");
        }
    }


}