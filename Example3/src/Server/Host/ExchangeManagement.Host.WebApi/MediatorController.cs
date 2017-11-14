using System;
using System.Net;
using System.Web.Http;
using MediatR;
using Swashbuckle.Swagger.Annotations;

namespace ExchangeManagement.Host.WebApi
{
    [SwaggerResponseRemoveDefaults]
    [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized")]
    public abstract class MediatorController : ApiController
    {
        protected IMediator Mediator { get; private set; }

        protected MediatorController(IMediator mediator)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            Mediator = mediator;
        }
    }
}