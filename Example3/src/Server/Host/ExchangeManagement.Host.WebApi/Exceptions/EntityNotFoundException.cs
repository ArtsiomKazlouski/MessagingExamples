using System;

namespace ExchangeManagement.Host.WebApi.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message)
            : base(message) { }
    }
}