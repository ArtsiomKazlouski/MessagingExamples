using System.Collections.Generic;

namespace ExchangeManagement.Contract.ServiceContracts
{
    /// <summary>
    /// Сервис управления подписками
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Регистрация подписки
        /// </summary>
        /// <param name="request">Характеристики подписки</param>
        /// <returns>Подписка</returns>
        Subscription Create(Contract.Subscription request);

        /// <summary>
        /// Удаление подписки по идентификатору
        /// </summary>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        /// <returns></returns>
        void Delete(long subscriptionId);

        /// <summary>
        /// Редактирование подписки
        /// </summary>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        /// <param name="request">Характеристики подписки</param>
        /// <returns></returns>
        Subscription Update(long subscriptionId, Contract.Subscription request);

        /// <summary>
        /// Получение подписки по идентификатору
        /// </summary>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        /// <returns></returns>
        Subscription GetById(long subscriptionId);

        /// <summary>
        /// Получение всех подписок
        /// </summary>
        /// <returns></returns>
        IList<Subscription> List();
    }
}
