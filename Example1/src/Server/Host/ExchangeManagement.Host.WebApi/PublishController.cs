using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Host.WebApi.PublishMessage;
using ExchangeManagement.Host.WebApi.Subscription;
using MediatR;
using Swashbuckle.Swagger.Annotations;

namespace ExchangeManagement.Host.WebApi
{
    public class PublishController: MediatorController
    {
        public PublishController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="message">Content</param>
        /// <returns>Подписка</returns>
        [HttpPost]
        [Route("", Name = "CreateMessage")]
        public async Task<IHttpActionResult> Create(MessageMetadata message)
        {
            await Mediator.SendAsync(new PublishMessageRequest(message));
            
            return Ok();
        }
    }
}