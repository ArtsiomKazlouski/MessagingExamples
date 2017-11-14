using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeManagement.Host.WebApi.SignalR;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Handlers
{
    public class SignalRHandler<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _inner;
        private readonly IEnumerable<IMessageProcessor<TRequest, TResponse>> _processors;


        public SignalRHandler(IAsyncRequestHandler<TRequest, TResponse> inner, IEnumerable<IMessageProcessor<TRequest,TResponse>> processors)
        {
            _inner = inner;
            _processors = processors;
        }

        async Task<TResponse> IAsyncRequestHandler<TRequest, TResponse>.Handle(TRequest request)
        {
            foreach (var messageProcessor in _processors)
            {
                await messageProcessor.OnBeforeRequestAsync(request);
            }

            var result = await _inner.Handle(request);

            foreach (var messageProcessor in _processors)
            {
                messageProcessor.OnRequestHandled(request, result);
            }
            return result;
        }
    }
}