using System;
using System.Data;
using System.Threading.Tasks;

namespace EHR.Cds.Infrastructure
{
    public interface IUnitOfWork: IDisposable
    {
        void PrventCommit();
        Task<TResult> QueryAsync<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> queryAsync);
    }
}