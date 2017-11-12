using System;

namespace ExchangeManagement.Contract
{
    /// <summary>
    /// Подписка
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// ID подписки
        /// </summary>
        public long SubscriptionId { get; set; }

        /// <summary>
        /// URL подписчика
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Поисковой запрос
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Для авторизации
        /// </summary>
        public AuthorizationOptions AuthorizationOptions { get; set; }

        /// <summary>
        /// Дата создания подписки
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата последнего обновления подписки
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Флаг для скачивания ресурсного файла
        /// </summary>
        public bool IsDownloadResourceFile { get; set; }

        /// <summary>
        /// Флаг активна ли подписка
        /// </summary>
        public bool IsActive { get; set; }
    }
}
