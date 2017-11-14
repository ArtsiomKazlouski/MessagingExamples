using System;

namespace ExchangeManagement.Host.WebApi.Exceptions
{
    public class EntityBadRequestException : Exception
    {
        public EntityBadRequestException(string message)
            : base(message) { }
    }
}