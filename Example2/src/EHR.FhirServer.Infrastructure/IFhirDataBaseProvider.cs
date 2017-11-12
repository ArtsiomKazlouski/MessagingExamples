using System;
using System.Data;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Npgsql;

namespace EHR.FhirServer.Infrastructure
{
    /// <summary>
    ///     UnitOfWork forFhirBase
    /// </summary>
    public interface IFhirDataBaseProvider : IDisposable
    {
        Resource WithConnection(string procedureName, params NpgsqlParameter[] npgsqlParameter);

        TResult WithConnection<TResult>(string procedureName, params NpgsqlParameter[] npgsqlParameter);

        void PeventCommit();

    }

    public class FhirDataBaseProvider : IFhirDataBaseProvider
    {
        private readonly Func<NpgsqlConnection> _connectionFactory;
        private NpgsqlConnection _dbConnection;
        private bool _disposed;
        private bool _preventCommit;
        private NpgsqlTransaction _transaction;

        public FhirDataBaseProvider(Func<NpgsqlConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _preventCommit = false;
        }

        public Resource WithConnection(string procedureName, params NpgsqlParameter[] npgsqlParameter)
        {
            return FhirParser.ParseResourceFromJson(WithConnection<string>(procedureName, npgsqlParameter));
        }

        public TResult WithConnection<TResult>(string procedureName, params NpgsqlParameter[] npgsqlParameter)
        {
            try
            {
                OpenConnection();

                return (TResult) BuildCommand(procedureName, CommandType.StoredProcedure, npgsqlParameter)
                    .ExecuteScalar();
            }
            catch (Exception)
            {
                _preventCommit = true;
                throw;
            }
        }


        

        private void OpenConnection()
        {
            if (_dbConnection == null)
            {
                _dbConnection = _connectionFactory();
                _dbConnection.Open();
            }
            if (_transaction == null)
                _transaction = _dbConnection.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void PeventCommit()
        {
            _preventCommit = true;
        }

        private IDbCommand BuildCommand(string procedureName, CommandType commandType,
            params NpgsqlParameter[] commandParams)
        {
            var command = new NpgsqlCommand(procedureName, _dbConnection, _transaction)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddRange(commandParams.ToArray());
            return command;
        }



        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            try
            {
                if (_transaction != null)
                {
                    if (_preventCommit)
                        _transaction.Rollback();
                    else
                        _transaction.Commit();
                }
                
            }
            catch
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    throw;
                }
            }
            finally
            {
                _transaction?.Dispose();
                _dbConnection?.Dispose();
            }
        }
    }
}