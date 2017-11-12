using System;

namespace ExchangeManagement.Host.WebApi.Exceptions
{
    public class EntityUpdateConcurrencyException : Exception
    {
        public EntityUpdateConcurrencyException(string message)
            : base(message) { }
    }
}