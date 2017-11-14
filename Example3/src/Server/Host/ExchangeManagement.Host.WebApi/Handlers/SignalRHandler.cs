using System;
using System.Threading.Tasks;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Handlers
{
    public class SignalRHandler<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _inner;
        

        public SignalRHandler(IAsyncRequestHandler<TRequest, TResponse> inner)
        {
            _inner = inner;
        }

        async Task<TResponse> IAsyncRequestHandler<TRequest, TResponse>.Handle(TRequest message)
        {
            return await _inner.Handle(message);
        }
    }
}