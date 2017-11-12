using System.Threading.Tasks;
using Dapper;
using ExchangeManagement.Host.WebApi.Exceptions;
using FluentValidation;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Subscription
{
    public class DeleteSubscriptionRequest : IAsyncRequest
    {
        public long SubscriptionId { get; set; }
    }

    public class DeleteSubscriptionRequestValidator : AbstractValidator<DeleteSubscriptionRequest>
    {
        public DeleteSubscriptionRequestValidator()
        {
            RuleFor(p => p.SubscriptionId).Must(y => (y > 0)).WithMessage("Идентификатор должен быть больше 0");
        }
    }

    public class DeleteSubscriptionHandler : AsyncRequestHandler<DeleteSubscriptionRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSubscriptionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task HandleCore(DeleteSubscriptionRequest message)
        {
            object param = new
            {
                sequenceId = message.SubscriptionId
            };
            
            var sql = "DELETE FROM subscription WHERE id = @sequenceId;";
            
            var deleted = await _unitOfWork.QueryAsync(async (c, t) => await c.ExecuteAsync(sql: sql, param: param, transaction: t));

            if (deleted != 1)
            {
                throw new EntityNotFoundException($"Подписка с идентификатором [{message.SubscriptionId}] не существует");
            }
        }
    }
}