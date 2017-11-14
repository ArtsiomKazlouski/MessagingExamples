using System.Threading.Tasks;
using System.Web.Http;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Host.WebApi.Calculation;
using MediatR;

namespace ExchangeManagement.Host.WebApi
{
    [RoutePrefix("calculation")]
    public class CalculationController: MediatorController
    {
        public CalculationController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Create task
        /// </summary>
        /// <param name="taskArguments">Content</param>
        /// <returns>Подписка</returns>
        [HttpPost]
        [Route("", Name = "CreateTask")]
        public async Task<IHttpActionResult> Create(TaskArguments taskArguments)
        {
            return Ok(await Mediator.SendAsync(new RequestCalculationRequest(taskArguments)));
        }

        /// <summary>
        /// Set results
        /// </summary>
        /// <param name="taskArguments">Content</param>
        /// <returns>Подписка</returns>
        [HttpPost]
        [Route("result", Name = "SetTaskResult")]
        public async Task<IHttpActionResult> Create(TaskCalculationResult calculationResult)
        {
            await Mediator.SendAsync(new AppendCalculationResultRequest(calculationResult));

            return Ok();
        }

        /// <summary>
        /// Get results
        /// </summary>
        /// <param name="taskArguments">Content</param>
        /// <returns>Подписка</returns>
        [HttpGet]
        [Route("result", Name = "SetTaskResult")]
        public async Task<IHttpActionResult> Create(long taskId)
        {
            await Mediator.SendAsync(new GetCalculationResultRequest(taskId));

            return Ok();
        }
    }
}