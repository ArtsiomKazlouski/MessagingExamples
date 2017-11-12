using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using ExchangeManagement.Host.WebApi.Subscription;
using Swashbuckle.Swagger.Annotations;

namespace ExchangeManagement.Host.WebApi
{
    /// <summary>
    /// Сервис управления подписками
    /// </summary>
    [RoutePrefix("subscription")]
    public class SubscriptionController: MediatorController
    {
        public SubscriptionController(IMediator mediator)
            : base(mediator) { }

        /// <summary>
        /// Регистрация подписки
        /// </summary>
        /// <param name="request">Характеристики подписки</param>
        /// <returns>Подписка</returns>
        [HttpPost]
        [Route("", Name = "CreateSubscription")]
        [SwaggerResponse(HttpStatusCode.Created, "Возвращает созданную подписку", typeof(Contract.Subscription))]
        [SwaggerResponse(HttpStatusCode.NotFound, "NotFound")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "BadRequest")]
        [SwaggerResponse(HttpStatusCode.Conflict, "Conflict")]
        public async Task<IHttpActionResult> Create(Contract.Subscription request)
        {
            var result = await Mediator.SendAsync(new CreateSubscriptionRequest(request));

            var subscription = await Mediator.SendAsync(new GetByIdSubscriptionRequest(result));

            return CreatedAtRoute("GetSubscription", new { subscriptionId = result }, subscription);
        }

        /// <summary>
        /// Удаление подписки по идентификатору
        /// </summary>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{subscriptionId:long}", Name = "DeleteSubscription")]
        [SwaggerResponse(HttpStatusCode.NoContent, "NoContent")]
        [SwaggerResponse(HttpStatusCode.NotFound, "NotFound")]
        public async Task<IHttpActionResult> Delete(long subscriptionId)
        {
            await Mediator.SendAsync(new DeleteSubscriptionRequest() { SubscriptionId = subscriptionId });
            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }

        /// <summary>
        /// Редактирование подписки
        /// </summary>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        /// <param name="request">Характеристики подписки</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{subscriptionId:long}", Name = "UpdateSubscription")]
        [SwaggerResponse(HttpStatusCode.OK, "Возвращает отредактированную подписку", typeof(Contract.Subscription))]
        [SwaggerResponse(HttpStatusCode.NotFound, "NotFound")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "BadRequest")]
        public async Task<IHttpActionResult> Update(long subscriptionId, Contract.Subscription request)
        {
            request.SubscriptionId = subscriptionId;
            await Mediator.SendAsync(new UpdateSubscriptionRequest(request));
            var subscription = await Mediator.SendAsync(new GetByIdSubscriptionRequest(subscriptionId));

            return Ok(subscription);
        }

        /// <summary>
        /// Получение подписки по идентификатору
        /// </summary>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{subscriptionId:long}", Name = "GetSubscription")]
        [SwaggerResponse(HttpStatusCode.OK, "Возвращает подписку", typeof(Contract.Subscription))]
        [SwaggerResponse(HttpStatusCode.NotFound, "NotFound")]
        public async Task<IHttpActionResult> Get(long subscriptionId)
        {
            var subscription = await Mediator.SendAsync(new GetByIdSubscriptionRequest(subscriptionId));
            return Ok(subscription);
        }

        /// <summary>
        /// Получение всех подписок
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("", Name = "ListSubscriptions")]
        [SwaggerResponse(HttpStatusCode.OK, "Возвращает список подписок", typeof(IEnumerable<Contract.Subscription>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "NotFound")]
        public async Task<IHttpActionResult> List()
        {
            var subscription = await Mediator.SendAsync(new ListSubscriptionsRequest());
            return Ok(subscription);
        }
    }
}