using System;
using System.Threading.Tasks;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Handlers
{
    public class SqlExceptionHandler<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _inner;
        private readonly IUnitOfWork _dbConnectionProvider;

        public SqlExceptionHandler(IAsyncRequestHandler<TRequest, TResponse> inner,
            IUnitOfWork dbConnectionProvider)
        {
            _inner = inner;
            _dbConnectionProvider = dbConnectionProvider;
        }

        async Task<TResponse> IAsyncRequestHandler<TRequest, TResponse>.Handle(TRequest message)
        {
            try
            {
                return await _inner.Handle(message);
            }
            catch (Exception exception)
            {
                _dbConnectionProvider.PrventCommit();
                throw exception;
            }
        }
    }
}