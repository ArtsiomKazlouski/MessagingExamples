using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExchangeManagement.Host.WebApi.SignalR
{
    public interface IMessageProcessor<TRequest, TResponce>
    {
        Task OnBeforeRequestAsync(TRequest request);
        void OnRequestHandled(TRequest request, TResponce responce);
    }
}