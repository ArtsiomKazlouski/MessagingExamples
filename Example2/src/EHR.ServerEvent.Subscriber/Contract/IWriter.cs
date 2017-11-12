using System.Threading.Tasks;

namespace EHR.ServerEvent.Subscriber.Contract
{
    public interface IWriter<in T>
    {
        Task WriteAsync(T data);
    }
}
