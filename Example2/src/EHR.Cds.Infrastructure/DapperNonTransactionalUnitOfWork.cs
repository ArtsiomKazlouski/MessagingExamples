using System;
using System.Data;
using System.Threading.Tasks;

namespace EHR.Cds.Infrastructure
{
    public class DapperNonTransactionalUnitOfWork : IUnitOfWork
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public DapperNonTransactionalUnitOfWork(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void PrventCommit()
        {
            throw new NotImplementedException("this unit of work does not support transaction");
        }

        public async Task<TResult> QueryAsync<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> queryAsync)
        {
            using (var dbConnection = _connectionFactory())
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var res = await queryAsync(dbConnection, trans);
                    return res;
                }
            }
        }

        public void Dispose(){}
    }
}