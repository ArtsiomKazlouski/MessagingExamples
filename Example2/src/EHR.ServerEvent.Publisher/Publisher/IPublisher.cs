using System;
using System.Threading.Tasks;
using EHR.ServerEvent.Infrastructure.Contract;

namespace EHR.ServerEvent.Publisher.Publisher
{
    public interface IPublisher
    {
        Task SendServerEventAsync(ServerEventMessage msg);
    }
}
