using System;

namespace EHR.ServerEvent.Publisher.Dequeue.Postgres
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(Message message)
        {
            Message = message;
        }

        public Message Message { get;  }
    }
}