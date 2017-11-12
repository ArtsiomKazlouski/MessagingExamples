namespace ExchangeManagement.Contract
{
    /// <summary>
    /// Для авторизации
    /// </summary>
    public class AuthorizationOptions
    {
        /// <summary>
        /// Url где получать токен
        /// </summary>
        public string TokenEndpoint { get; set; }

        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Секретное слово
        /// </summary>
        public string ClientSecret { get; set; }
    }
}
