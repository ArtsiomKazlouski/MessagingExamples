using System;

namespace EHR.ServerEvent.Publisher.Dequeue.Postgres
{
    public class PoolingOptions
    {
        public PoolingOptions():this("server_event")
        {
            
        }

        public PoolingOptions(string tableName):
            this(
                tableName:tableName,
                poolInterval: TimeSpan.FromSeconds(15),
                invisibilityTimeout: TimeSpan.FromMinutes(3),
                schemaName: "queue")
        {
        }

        public PoolingOptions(string tableName, TimeSpan poolInterval, TimeSpan invisibilityTimeout, string schemaName)
        {
            ThrowIfValueIsNotPositive(poolInterval,nameof(poolInterval));
            ThrowIfValueIsNotPositive(invisibilityTimeout,nameof(invisibilityTimeout));
            if (string.IsNullOrWhiteSpace(schemaName))
            {
                throw new ArgumentNullException(nameof(schemaName));
            }
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            QueuePollInterval = poolInterval;
            InvisibilityTimeout = invisibilityTimeout;
            SchemaName = schemaName;
            TableName = tableName;

        }

        public TimeSpan QueuePollInterval { get; set; }
        public TimeSpan InvisibilityTimeout { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }

        private static void ThrowIfValueIsNotPositive(TimeSpan value, string fieldName)
        {
            var message = $"The {fieldName} property value should be positive. Given: {value}.";

            if (value == TimeSpan.Zero)
            {
                throw new ArgumentException(message, nameof(value));
            }
            if (value != value.Duration())
            {
                throw new ArgumentException(message, nameof(value));
            }
        }
    }
}