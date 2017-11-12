using System;
using System.Data;
using System.Threading.Tasks;

namespace ExchangeManagement.Host.WebApi
{
    public interface IUnitOfWork
    {
        void PrventCommit();
        Task<TResult> QueryAsync<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> queryAsync);
    }
}