using System;
using System.Data;
using System.Threading.Tasks;

namespace EHR.Cds.Infrastructure
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _transaction;
        private bool _preventCommit;
        private bool _disposed;

        public DapperUnitOfWork(Func<IDbConnection> connectionFactory)
        {
            _dbConnection = connectionFactory();
            _dbConnection.Open();
            _transaction = _dbConnection.BeginTransaction(IsolationLevel.ReadCommitted);
        }



        public void PrventCommit()
        {
            _preventCommit = true;
        }

        public async Task<TResult> QueryAsync<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> queryAsync)
        {
            try
            {
                return await queryAsync(_dbConnection, _transaction);
            }
            catch (Exception)
            {
                PrventCommit();
                throw;
            }

        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                if (_preventCommit)
                {
                    _transaction.Rollback();
                }
                else
                {
                    _transaction.Commit();
                }
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _dbConnection.Dispose();
            }



        }
    }
}
