using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EasyNetQ;
using EasyNetQ.Topology;
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
        private readonly IAdvancedBus _advancedBus;

        public PublishMessageHandler(IAdvancedBus advancedBus)
        {
            _advancedBus = advancedBus;
        }

        protected override async Task HandleCore(PublishMessageRequest message)
        {
            var exchanger = _advancedBus.ExchangeDeclare(ExchangerNames.InformationResource, ExchangeType.Topic);

            await _advancedBus.PublishAsync(exchanger, MessageTopics.Created,false,new Message<MessageMetadata>(message.MessageMetadata));
        }
    }


}