using System;

namespace EHR.ServerEvent.Subscriber.Contract
{
    public interface IMessageProcessor<TSrc>:IDisposable
    {
        event ProcessHandler<TSrc> OnProcessed;
        event ErrorHandler<TSrc> OnProcessError;
        void Start();
        void Stop();
    }

    public delegate void ProcessHandler<T>(IMessageProcessor<T> m, MessageProcessedEventArgs<T> e);
    public delegate void ErrorHandler<T>(IMessageProcessor<T> m, MessageProcessErrorEventArgs e);

    public class MessageProcessErrorEventArgs : EventArgs
    {
        public Exception Error { get; }

        public MessageProcessErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }

    public class MessageProcessedEventArgs<T> : EventArgs
    {
        public T Message { get;  }

        public MessageProcessedEventArgs(T message)
        {
            Message = message;
        }
    }
}
