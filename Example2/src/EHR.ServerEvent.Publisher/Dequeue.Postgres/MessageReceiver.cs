using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace EHR.ServerEvent.Publisher.Dequeue.Postgres
{
    public class MessageReceiver : IMessageReceiver, IDisposable
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private readonly string _deleteQuery;

        private readonly object _lockObject = new object();
        private readonly ILogger _logger;


        private readonly PoolingOptions _poolingOptions;
        private readonly string _readQuery;
        private readonly string _updateQuery;
        private CancellationTokenSource _cancellationSource;


        public MessageReceiver(Func<IDbConnection> connectionFactory, PoolingOptions options,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _connectionFactory = connectionFactory;
            _poolingOptions = options;

            _readQuery =
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"
                        WITH RECURSIVE events AS (
                          SELECT (e).*, pg_try_advisory_lock((e).id) AS locked
                          FROM (
                            SELECT e
                            FROM {0} AS e
                            WHERE (fetched_at IS NULL) OR (fetched_at <=  NOW()  + INTERVAL '{1} SECONDS') and 
                                    id not in (select objid from pg_locks where locktype = 'advisory')
                            ORDER BY created_at asc
                            LIMIT 1
                          ) AS e1
                          UNION ALL (
                            SELECT (e).*, pg_try_advisory_lock((e).id) AS locked
                            FROM (
                              SELECT (
                                SELECT e
                                FROM {0} AS e
                                WHERE ((fetched_at IS NULL) OR (fetched_at <=  NOW()  + INTERVAL '{1} SECONDS'))
                                AND created_at > events.created_at and 
                                    id not in (select objid from pg_locks where locktype = 'advisory')
                                ORDER BY created_at asc
                                LIMIT 1
                              ) AS e
                              FROM events
                              LIMIT 1
                            ) AS e1
                          )
                        )
                        SELECT id as Id, resource as Body, created_at as CreatedAt
                        FROM events
                        WHERE locked
                        LIMIT 1",
                    $"{options.SchemaName}.{options.TableName}",
                    _poolingOptions.InvisibilityTimeout.Negate().TotalSeconds);
            _deleteQuery = $"DELETE FROM {options.SchemaName}.{options.TableName} WHERE id = @Id";

            _updateQuery =
                $"UPDATE {_poolingOptions.SchemaName}.{_poolingOptions.TableName} SET fetched_at = NOW() WHERE id = @Id";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived = (sender, args) => { };

        public void Start()
        {
            lock (_lockObject)
            {
                if (_cancellationSource == null)
                {
                    _cancellationSource = new CancellationTokenSource();
                    Task.Factory.StartNew(
                        () => ReceiveMessages(_cancellationSource.Token),
                        _cancellationSource.Token,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Current);
                }
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                using (_cancellationSource)
                {
                    if (_cancellationSource != null)
                    {
                        _cancellationSource.Cancel();
                        _cancellationSource = null;
                    }
                }
            }
        }

        public event EventHandler<EventArgs> RecievingStopped = (sender, args) => { };

        protected virtual void Dispose(bool disposing)
        {
            Stop();
        }

        ~MessageReceiver()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Receives the messages in an endless loop.
        /// </summary>
        private void ReceiveMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
                if (!ReceiveMessage())
                    cancellationToken.WaitHandle.WaitOne(_poolingOptions.QueuePollInterval);
            RecievingStopped?.Invoke(this, EventArgs.Empty);
        }

        protected bool ReceiveMessage()
        {
            try
            {
                _logger.LogInformation("Try to get message from database queue");
                using (var connection = _connectionFactory.Invoke())
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        var message = connection.Query<Message>(_readQuery, new { }, transaction)
                            .SingleOrDefault();

                        if (message == null)
                        {
                            _logger.LogInformation("Empty database queue");
                            return false;
                        }
                        _logger.LogInformation("Message got from database queue");
                        try
                        {
                            _logger.LogInformation("Send message to rabbitMQ");
                            MessageReceived(this, new MessageReceivedEventArgs(message));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(0, ex, "Произошла ошибка при отправки сообщения");

                            connection.Execute(_updateQuery, new {message.Id}, transaction);
                            transaction.Commit();
                            return true;
                        }
                        _logger.LogInformation("Delete message from database queue");

                        connection.Execute(_deleteQuery, new {message.Id}, transaction);

                        transaction.Commit();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Get message error");
                return false;
            }
        }
    }
}